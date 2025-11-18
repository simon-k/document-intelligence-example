using FitnessExercises.Analyzer.Models;

namespace FitnessExercises.Analyzer;

public interface IAnalyzer
{
    public Task<Exercise> AnalyzeAsync(string filePath);
    public Task<Exercise> AnalyzeAsync(Uri uri);
    public Task<Exercise> AnalyzeAsync(Stream stream);
}