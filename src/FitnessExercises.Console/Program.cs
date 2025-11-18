using FitnessExercises.Analyzer;

var adiEndpoint = Environment.GetEnvironmentVariable("AzureDocumentIntelligence:Endpoint") ?? throw new InvalidOperationException("AzureDocumentIntelligence:Endpoint is not configured");
var adiKey = Environment.GetEnvironmentVariable("AzureDocumentIntelligence:Key") ?? throw new InvalidOperationException("AzureDocumentIntelligence:Key is not configured");
var adiModelId = Environment.GetEnvironmentVariable("AzureDocumentIntelligence:ModelId") ?? throw new InvalidOperationException("AzureDocumentIntelligence:ModelId is not configured");

var acuEndpoint = Environment.GetEnvironmentVariable("AzureContentUnderstanding:Endpoint") ?? throw new InvalidOperationException("AzureContentUnderstanding:Endpoint is not configured");
var acuKey = Environment.GetEnvironmentVariable("AzureContentUnderstanding:Key") ?? throw new InvalidOperationException("AzureContentUnderstanding:Key is not configured");
var acuAnalyzerId = Environment.GetEnvironmentVariable("AzureContentUnderstanding:AnalyzerId") ?? throw new InvalidOperationException("AzureContentUnderstanding:AnalyzerId is not configured");

Console.WriteLine("1) Azure AI Document Intelligence");
Console.WriteLine("2) Azure AI Foundry Content Understanding");
Console.Write("Choose analyzer (1/2): ");
var choice = Console.ReadLine();

IAnalyzer analyzer = choice == "1"
    ? new DocumentIntelligenceAnalyzer(adiEndpoint, adiKey, adiModelId)
    : new ContentUnderstandingAnalyzer(acuEndpoint, acuKey, acuAnalyzerId);

var exercise = await analyzer.AnalyzeAsync("data/dummy exercises - arm day 1.jpg");

var exercisesJson = System.Text.Json.JsonSerializer.Serialize(exercise, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
Console.WriteLine($"Result:{Environment.NewLine}{exercisesJson}");
