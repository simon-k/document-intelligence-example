using Azure;
using Azure.AI.DocumentIntelligence;

namespace FitnessExercises.Analyzer;

public class FileAnalyzer : IAnalyzer
{
    private readonly DocumentIntelligenceClient _client;
    private readonly string _modelId;

    public FileAnalyzer(string endpoint, string key, string modelId)
    {
        _client = new DocumentIntelligenceClient(new Uri(endpoint), new AzureKeyCredential(key));
        _modelId = modelId;
    }
    
    public async Task<AnalyzeResult> Analyze()
    {
        var filePath = "data/dummy exercises - arm day 1.jpg";
        Console.WriteLine($"Analyzing file {filePath}...");
        using var stream = File.OpenRead(filePath);
        var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, _modelId, BinaryData.FromStream(stream));
        return operation.Value;
    }
}