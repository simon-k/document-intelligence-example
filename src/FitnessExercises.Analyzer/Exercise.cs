namespace FitnessExercises.Analyzer;

public class Exercise
{
    public string Type { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public List<Phase> Phases { get; set; } = [];
}

public class Phase
{
    public string Name { get; set; } = string.Empty;
    public List<string> Tasks { get; set; } = [];
}
