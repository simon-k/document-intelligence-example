using Azure;
using Azure.AI.DocumentIntelligence;

namespace FitnessExercises.Analyzer;

public class UrlAnalyzer : IAnalyzer
{
    private readonly DocumentIntelligenceClient _client;
    private readonly string _modelId;

    public UrlAnalyzer(string endpoint, string key, string modelId)
    {
        _client = new DocumentIntelligenceClient(new Uri(endpoint), new AzureKeyCredential(key));
        _modelId = modelId;
    }

    public async Task<AnalyzeResult> Analyze()
    {
        // URL Source
        var fileUri = new Uri("https://raw.githubusercontent.com/simon-k/document-intelligence-example/refs/heads/main/data/dummy%20exercises%20-%20arm%20day%201.jpg");
        Console.WriteLine($"Analyzing URL {fileUri}...");
        var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, _modelId, fileUri);
        return operation.Value;
    }

    public Task<AnalyzeResult> Analyze(DocumentIntelligenceClient client, string modelId)
    {
        throw new NotImplementedException();
    }
}