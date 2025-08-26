using Azure;
using Azure.AI.DocumentIntelligence;
using FitnessExercises.Console;
using FitnessExercises.Console.Analyzer;

var endpoint = Environment.GetEnvironmentVariable("AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT") ?? throw new InvalidOperationException("Environment variable AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT is not set.");
var key = Environment.GetEnvironmentVariable("AZURE_DOCUMENT_INTELLIGENCE_KEY") ?? throw new InvalidOperationException("Environment variable AZURE_DOCUMENT_INTELLIGENCE_KEY is not set.");

var client = new DocumentIntelligenceClient(new Uri(endpoint), new AzureKeyCredential(key));
var modelId = "exercise-extractor-model";

Console.Write("Analyze file or URL? (f/u): ");
var choice = Console.ReadLine()?.Trim().ToLower();

var analyzer = (choice == "f") ? 
    AnalyzerFactory.CreateAnalyzer("file")  : 
    AnalyzerFactory.CreateAnalyzer("url");

var analyzeResult = await analyzer.Analyze(client, modelId);

var exercise = new Exercise();
// Find fields in the analyzed documents
foreach (var document in analyzeResult.Documents)
{
    exercise.Type = document.Fields["TrainingType"]?.Content ?? string.Empty;
    exercise.Duration = document.Fields["Duration"]?.Content ?? string.Empty;
}

// Find tables in the analyzed document
foreach (var table in analyzeResult.Tables)
{
    var phasesIndex = -1;
    foreach (var cell in table.Cells)
    {
        if (string.IsNullOrWhiteSpace(cell.Content)) continue; // Ignore empty cells
        
        switch (cell.ColumnIndex)
        {
            case 0:
                exercise.Phases.Add(new Phase { Name = cell.Content });
                phasesIndex++;
                break;
            case 1:
                if (phasesIndex < 0) continue; // No phase found yet
                exercise.Phases[phasesIndex].Tasks.Add(cell.Content);
                break;
        }
    }
}

var exercisesJson = System.Text.Json.JsonSerializer.Serialize(exercise, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
Console.WriteLine($"Result:{Environment.NewLine}{exercisesJson}");
