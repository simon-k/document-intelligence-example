using Azure.AI.DocumentIntelligence;

namespace FitnessExercises.Analyzer;

public static class ExerciseMapper
{
    public static Exercise Map(AnalyzeResult analyzeResult)
    {
        var exercise = new Exercise();
        
        // Find fields in the analyzed documents
        foreach (var document in analyzeResult.Documents)
        {
            exercise.Type = document.Fields["TrainingType"]?.Content ?? string.Empty;
            exercise.Duration = document.Fields["Duration"]?.Content ?? string.Empty;
        }

        // Find tables in the analyzed document
        foreach (var table in analyzeResult.Tables)
        {
            var phasesIndex = -1;
            foreach (var cell in table.Cells)
            {
                if (string.IsNullOrWhiteSpace(cell.Content)) continue; // Ignore empty cells
        
                switch (cell.ColumnIndex)
                {
                    case 0:
                        exercise.Phases.Add(new Phase { Name = cell.Content });
                        phasesIndex++;
                        break;
                    case 1:
                        if (phasesIndex < 0) continue; // No phase found yet
                        exercise.Phases[phasesIndex].Tasks.Add(cell.Content);
                        break;
                }
            }
        }

        return exercise;
    }
}
