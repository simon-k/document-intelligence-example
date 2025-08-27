using Azure.AI.DocumentIntelligence;
using FitnessExercises.Analyzer;

// Put this in a config...
var endpoint = Environment.GetEnvironmentVariable("AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT") ?? throw new InvalidOperationException("Environment variable AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT is not set.");
var key = Environment.GetEnvironmentVariable("AZURE_DOCUMENT_INTELLIGENCE_KEY") ?? throw new InvalidOperationException("Environment variable AZURE_DOCUMENT_INTELLIGENCE_KEY is not set.");
var modelId = "exercise-extractor-model";

var analyzer = new Analyzer(endpoint, key, modelId);

Console.Write("Analyze file or URL? (f/u): ");
var choice = Console.ReadLine()?.Trim().ToLower() == "f" ? "file" : "url";

var analyzeResult = await AnalyzeAsync(choice);
var exercise = ExerciseMapper.Map(analyzeResult);

var exercisesJson = System.Text.Json.JsonSerializer.Serialize(exercise, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
Console.WriteLine($"Result:{Environment.NewLine}{exercisesJson}");

return;

async Task<AnalyzeResult> AnalyzeAsync(string choice)
{
    return choice switch
    {
        "file" => await analyzer.AnalyzeAsync("data/dummy exercises - arm day 1.jpg"),
        "url" => await analyzer.AnalyzeAsync(new Uri("https://raw.githubusercontent.com/simon-k/document-intelligence-example/refs/heads/main/data/dummy%20exercises%20-%20arm%20day%201.jpg")),
        _ => throw new ArgumentException("Invalid choice. Please enter 'f' for file or 'u' for URL.")
    };
}
