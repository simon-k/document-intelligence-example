namespace FitnessExercises.Console.Analyzer;

public static class AnalyzerFactory
{
    public static IAnalyzer CreateAnalyzer(string choice) => choice.ToLower() switch
    {
        "url" => new UrlAnalyzer(),
        "file" => new FileAnalyzer(),
        _ => throw new ArgumentException("Invalid choice. Please select 'url' or 'file'.")
    };
}