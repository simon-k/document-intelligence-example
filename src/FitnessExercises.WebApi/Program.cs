var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();

app.MapPost("/upload", async (IFormFile file) =>
{
    if (file.Length == 0)  //TODO: TODO: Handle large files. The doc intelligence must have a limit.
    {
        return Results.BadRequest("No file uploaded or file is empty.");
    }
    
    //TODO: Better way to handle stream?  
    await using var memoryStream = new MemoryStream();
    await file.CopyToAsync(memoryStream);
    memoryStream.Position = 0; // Reset position if further processing is needed

    // Here you could pass 'memoryStream' to downstream processing (e.g., analyzer)
    // For now, just return basic info.
    return Results.Ok(new { file.FileName, Length = memoryStream.Length });
    
}).DisableAntiforgery(); // Disable antiforgery for API endpoint. Not good for production without proper security.

app.Run();
