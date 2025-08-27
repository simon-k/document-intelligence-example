using FitnessExercises.Analyzer;

var endpoint = Environment.GetEnvironmentVariable("AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT") ?? throw new InvalidOperationException("Environment variable AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT is not set.");
var key = Environment.GetEnvironmentVariable("AZURE_DOCUMENT_INTELLIGENCE_KEY") ?? throw new InvalidOperationException("Environment variable AZURE_DOCUMENT_INTELLIGENCE_KEY is not set.");
var modelId = "exercise-extractor-model";

Console.Write("Analyze file or URL? (f/u): ");
var choice = Console.ReadLine()?.Trim().ToLower() == "f" ? "file" : "url";

var analyzer = AnalyzerFactory.CreateAnalyzer(choice, endpoint, key, modelId);
var analyzeResult = await analyzer.Analyze();

var exercise = ExerciseMapper.Map(analyzeResult);

var exercisesJson = System.Text.Json.JsonSerializer.Serialize(exercise, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
Console.WriteLine($"Result:{Environment.NewLine}{exercisesJson}");
