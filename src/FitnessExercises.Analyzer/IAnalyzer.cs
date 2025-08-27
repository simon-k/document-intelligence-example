using Azure.AI.DocumentIntelligence;

namespace FitnessExercises.Analyzer;

public interface IAnalyzer
{
    Task<AnalyzeResult> Analyze();
}