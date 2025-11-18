namespace FitnessExercises.Analyzer.Models;

using System.Text.Json.Serialization;

public class AnalyzerResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; }
    
    [JsonPropertyName("result")]
    public Result Result { get; set; }
}

public class Result
{
    [JsonPropertyName("analyzerId")]
    public string AnalyzerId { get; set; }
    
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("contents")]
    public List<Content> Contents { get; set; }
}

public class Content
{
    [JsonPropertyName("markdown")]
    public string Markdown { get; set; }
    
    [JsonPropertyName("fields")]
    public Fields Fields { get; set; }
}

public class Fields
{
    [JsonPropertyName("Day")]
    public Field Day { get; set; }
    
    [JsonPropertyName("Duration")]
    public Field Duration { get; set; }
    
    [JsonPropertyName("Exercises")]
    public ExercisesField Exercises { get; set; }
}

public class Field
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("valueString")]
    public string ValueString { get; set; }
    
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }
}

public class ExercisesField
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("valueArray")]
    public List<ExerciseItem> ValueArray { get; set; }
}

public class ExerciseItem
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("valueObject")]
    public ExerciseValue ValueObject { get; set; }
}

public class ExerciseValue
{
    [JsonPropertyName("Phase")]
    public Field Phase { get; set; }
    
    [JsonPropertyName("Exercise")]
    public Field Exercise { get; set; }
}