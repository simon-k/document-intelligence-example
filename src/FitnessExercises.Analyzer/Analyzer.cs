using Azure;
using Azure.AI.DocumentIntelligence;

namespace FitnessExercises.Analyzer;

public class Analyzer(string endpoint, string key, string modelId)
{
    private readonly DocumentIntelligenceClient _client = new(new Uri(endpoint), new AzureKeyCredential(key));

    public async Task<AnalyzeResult> AnalyzeAsync(string filePath)
    {
        Console.WriteLine($"Analyzing file {filePath}...");
        using var stream = File.OpenRead(filePath);
        var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, BinaryData.FromStream(stream));
        return operation.Value;
    }
    
    public async Task<AnalyzeResult> AnalyzeAsync(Uri uri)
    {
        Console.WriteLine($"Analyzing URL {uri}...");
        var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, uri);
        return operation.Value;
    }
    
    public async Task<AnalyzeResult> AnalyzeAsync(Stream stream)
    {
        Console.WriteLine("Analyzing stream...");
        //TODO Check length. Doc intelligence has a limit.
        var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, BinaryData.FromStream(stream));
        return operation.Value;
    }
}