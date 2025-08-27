namespace FitnessExercises.Analyzer;

public static class AnalyzerFactory
{
    public static IAnalyzer CreateAnalyzer(string choice, string endpoint, string key, string modelId) =>
        choice.ToLower() switch
        {
            "url" => new UrlAnalyzer(endpoint, key, modelId),
            "file" => new FileAnalyzer(endpoint, key, modelId),
            _ => throw new ArgumentException("Invalid choice. Please select 'url' or 'file'.")
        };
}