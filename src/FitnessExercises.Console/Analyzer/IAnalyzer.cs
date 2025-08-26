using Azure.AI.DocumentIntelligence;

namespace FitnessExercises.Console.Analyzer;

public interface IAnalyzer
{
    Task<AnalyzeResult> Analyze(DocumentIntelligenceClient client, string modelId);
}