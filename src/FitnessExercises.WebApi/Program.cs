using FitnessExercises.Analyzer;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

var endpoint = app.Configuration["AzureDocumentIntelligence:Endpoint"] ?? throw new InvalidOperationException("AzureDocumentIntelligence:Endpoint is not configured.");
var key = app.Configuration["AzureDocumentIntelligence:Key"] ?? throw new InvalidOperationException("AzureDocumentIntelligence:Key is not configured.");
var modelId = app.Configuration["AzureDocumentIntelligence:ModelId"] ?? "exercise-extractor-model";


app.MapPost("/upload", async (HttpRequest request) =>
{
    if (!request.HasFormContentType)
    {
        return Results.BadRequest("Invalid content type.");
    }

    var form = await request.ReadFormAsync();
    var files = form.Files;

    if (files == null || files.Count == 0)
    {
        return Results.BadRequest("No files uploaded.");
    }

    var results = new List<object>();
    
    foreach (var file in files)
    {
        if (file.Length == 0)
        {
            results.Add(new { fileName = file.FileName, error = "File is empty." });
            continue;
        }
        
        try
        {
            //TODO: Better way to handle stream?  
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset position if further processing is needed

            // Here you could pass 'memoryStream' to downstream processing (e.g., analyzer)
            var analyser = new Analyzer(endpoint, key, modelId);
            var result = await analyser.AnalyzeAsync(memoryStream);
            var exercises = ExerciseMapper.Map(result);
            
            results.Add(new { fileName = file.FileName, exercises = exercises });
        }
        catch (Exception ex)
        {
            results.Add(new { fileName = file.FileName, error = ex.Message });
        }
    }
    
    return Results.Ok(results);
    
}).DisableAntiforgery(); // Disable antiforgery for API endpoint. Not good for production without proper security.

app.Run();
