using FitnessExercises.Analyzer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();

var endpoint = app.Configuration["AzureDocumentIntelligence:Endpoint"] ?? throw new InvalidOperationException("AzureDocumentIntelligence:Endpoint is not configured.");
var key = app.Configuration["AzureDocumentIntelligence:Key"] ?? throw new InvalidOperationException("AzureDocumentIntelligence:Key is not configured.");
var modelId = app.Configuration["AzureDocumentIntelligence:ModelId"] ?? "exercise-extractor-model";


app.MapPost("/upload", async (IFormFile file) =>
{
    if (file.Length == 0)  // TODO: Handle large files. The doc intelligence must have a limit.
    {
        return Results.BadRequest("No file uploaded or file is empty.");
    }
    
    //TODO: Better way to handle stream?  
    await using var memoryStream = new MemoryStream();
    await file.CopyToAsync(memoryStream);
    memoryStream.Position = 0; // Reset position if further processing is needed

    // Here you could pass 'memoryStream' to downstream processing (e.g., analyzer)
    // For now, just return basic info.
    var analyser = new Analyzer(endpoint, key, modelId);
    var result = await analyser.AnalyzeAsync(memoryStream);
    var exercises = ExerciseMapper.Map(result);
    
    return Results.Ok(exercises);
    
}).DisableAntiforgery(); // Disable antiforgery for API endpoint. Not good for production without proper security.

app.MapPost("/uploads", async (HttpRequest request) =>
{
    if (!request.HasFormContentType)
    {
        return Results.BadRequest("Request must be multipart/form-data.");
    }

    var form = await request.ReadFormAsync();
    var files = form.Files;
    //{
    //    return Results.BadRequest("No files uploaded.");
   // }

    var allExercises = new List<object>(); // Adjust type based on your exercise model
    var analyser = new Analyzer(endpoint, key, modelId);

    foreach (var file in files)
    {
        if (file.Length == 0)
        {
            continue; // Skip empty files
        }

        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var result = await analyser.AnalyzeAsync(memoryStream);
        var exercises = ExerciseMapper.Map(result);
        allExercises.AddRange(exercises);
    }

    return Results.Ok(allExercises);

}).DisableAntiforgery();

app.Run();
