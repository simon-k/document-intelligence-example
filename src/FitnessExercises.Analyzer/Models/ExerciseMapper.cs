using Azure.AI.DocumentIntelligence;

namespace FitnessExercises.Analyzer.Models;

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

    /// <summary>
    /// Maps a deserialized <see cref="AnalyzerResponse"/> (raw JSON response) into an <see cref="Exercise"/> domain model.
    /// </summary>
    /// <param name="analyzerResponse">The analyzer response object.</param>
    /// <returns>Populated <see cref="Exercise"/>.</returns>
    public static Exercise Map(AnalyzerResponse? analyzerResponse)
    {
        var exercise = new Exercise();
        if (analyzerResponse?.Result?.Contents == null || analyzerResponse.Result.Contents.Count == 0)
        {
            return exercise; // Empty
        }

        // Assuming first content holds the structured fields we care about
        var content = analyzerResponse.Result.Contents.First();
        var fields = content.Fields;

        exercise.Type = fields?.Day?.ValueString ?? string.Empty; // Treat Day as the Type (e.g., "Arm Day")
        exercise.Duration = fields?.Duration?.ValueString ?? string.Empty;

        var exercisesArray = fields?.Exercises?.ValueArray;
        if (exercisesArray == null || exercisesArray.Count == 0)
        {
            return exercise; // No phases / tasks
        }

        // Group exercises by Phase
        var phaseLookup = new Dictionary<string, Phase>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in exercisesArray)
        {
            var phaseName = item?.ValueObject?.Phase?.ValueString?.Trim();
            var task = item?.ValueObject?.Exercise?.ValueString?.Trim();
            if (string.IsNullOrWhiteSpace(phaseName)) continue; // Skip if no phase name

            if (!phaseLookup.TryGetValue(phaseName, out var phase))
            {
                phase = new Phase { Name = phaseName };
                phaseLookup[phaseName] = phase;
            }
            if (!string.IsNullOrWhiteSpace(task))
            {
                phase.Tasks.Add(task);
            }
        }

        // Preserve insertion order based on first occurrence in the array
        exercise.Phases = exercisesArray
            .Select(i => i?.ValueObject?.Phase?.ValueString?.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(n => phaseLookup[n!])
            .ToList();

        return exercise;
    }
}
