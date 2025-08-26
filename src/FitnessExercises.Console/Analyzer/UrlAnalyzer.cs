using Azure;
using Azure.AI.DocumentIntelligence;

namespace FitnessExercises.Console.Analyzer;

public class UrlAnalyzer : IAnalyzer
{
    public async Task<AnalyzeResult> Analyze(DocumentIntelligenceClient client, string modelId)
    {
        // URL Source
        var fileUri = new Uri("https://raw.githubusercontent.com/simon-k/document-intelligence-example/refs/heads/main/data/dummy%20exercises%20-%20arm%20day%201.jpg");
        System.Console.WriteLine($"Analyzing URL {fileUri}...");
        var operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, fileUri);
        return operation.Value;
    }
}