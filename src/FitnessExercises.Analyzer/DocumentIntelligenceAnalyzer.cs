using Azure;
using Azure.AI.DocumentIntelligence;
using FitnessExercises.Analyzer.Models;

namespace FitnessExercises.Analyzer;

public class DocumentIntelligenceAnalyzer(string endpoint, string key, string modelId) : IAnalyzer
{
    private readonly DocumentIntelligenceClient _client = new(new Uri(endpoint), new AzureKeyCredential(key));

    public async Task<Exercise> AnalyzeAsync(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, BinaryData.FromStream(stream));

        var exercise = ExerciseMapper.Map(operation.Value);
        
        return exercise;
    }
    
    public async Task<Exercise> AnalyzeAsync(Uri uri)
    {
        var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, uri);

        var exercise = ExerciseMapper.Map(operation.Value);
        
        return exercise;
    }
    
    public async Task<Exercise> AnalyzeAsync(Stream stream)
    {
        var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, BinaryData.FromStream(stream));
        
        var exercise = ExerciseMapper.Map(operation.Value);
        
        return exercise;
    }
}