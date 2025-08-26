using Azure;
using Azure.AI.DocumentIntelligence;

namespace FitnessExercises.Console.Analyzer;

public class FileAnalyzer : IAnalyzer
{
    public async Task<AnalyzeResult> Analyze(DocumentIntelligenceClient client, string modelId)
    {
        var filePath = "data/dummy exercises - arm day 1.jpg";
        System.Console.WriteLine($"Analyzing file {filePath}...");
        using var stream = File.OpenRead(filePath);
        var operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, BinaryData.FromStream(stream));
        return operation.Value;
    }
}