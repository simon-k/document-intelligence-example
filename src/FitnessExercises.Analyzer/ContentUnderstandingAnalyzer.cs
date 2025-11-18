using System.Net.Http.Headers;
using System.Net.Http.Json;
using FitnessExercises.Analyzer.Models;

namespace FitnessExercises.Analyzer;

public class ContentUnderstandingAnalyzer(string endpoint, string key, string analyzerId) : IAnalyzer
{
    private const string ApiVersion = "2025-05-01-preview";
    private readonly HttpClient _client = new();
    
    public async Task<Exercise> AnalyzeAsync(string filePath)
    {
        var fileBytes = await File.ReadAllBytesAsync(filePath);
        var content = new ByteArrayContent(fileBytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        
        var exercise = await GetResponseAsync(content);
        
        return exercise;
    }
    
    public async Task<Exercise> AnalyzeAsync(Uri uri)
    {
        throw new NotImplementedException();
    }
    
    public async Task<Exercise> AnalyzeAsync(Stream stream)
    {
        throw new NotImplementedException();
    }

    private async Task<Exercise> GetResponseAsync(HttpContent content)
    {
        // Request document analysis
        var postContentEndpoint = $"{endpoint}/analyzers/{analyzerId}:analyze?api-version={ApiVersion}";
        var request = new HttpRequestMessage(HttpMethod.Post, postContentEndpoint);
        request.Headers.Add("Ocp-Apim-Subscription-Key", key);
        request.Content = content;
        
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var analyzerResult = await response.Content.ReadFromJsonAsync<AnalyzerResponse>() ?? throw new InvalidOperationException("Response content is null");

        // Polling analysis result
        while (analyzerResult.Status is "Running" or "NotStarted")
        {
            await Task.Delay(1000); // Wait for 1 second before polling again
            var statusRequest = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}/analyzerResults/{analyzerResult.Id}?api-version={ApiVersion}");
            statusRequest.Headers.Add("Ocp-Apim-Subscription-Key", key);
            
            var statusResponse = await _client.SendAsync(statusRequest);
            statusResponse.EnsureSuccessStatusCode();

            analyzerResult = await statusResponse.Content.ReadFromJsonAsync<AnalyzerResponse>() ?? throw new InvalidOperationException("Response content is null");
            
            Console.WriteLine($"Request: {analyzerResult.Id} Status: {analyzerResult.Status}");
        }

        var exercise = ExerciseMapper.Map(analyzerResult);
        return exercise;
    }
}