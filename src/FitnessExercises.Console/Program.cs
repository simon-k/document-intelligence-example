using System.Reflection.Metadata.Ecma335;
using Azure;
using Azure.AI.DocumentIntelligence;

var endpoint = Environment.GetEnvironmentVariable("AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT") ?? throw new InvalidOperationException("Environment variable AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT is not set.");
var key = Environment.GetEnvironmentVariable("AZURE_DOCUMENT_INTELLIGENCE_KEY") ?? throw new InvalidOperationException("Environment variable AZURE_DOCUMENT_INTELLIGENCE_KEY is not set.");
var credential = new AzureKeyCredential(key);

// ListAvailableModels(endpoint, credential);

var client = new DocumentIntelligenceClient(new Uri(endpoint), credential);
var modelId = "exercise-extractor-model";
var fileUri = new Uri("https://raw.githubusercontent.com/simon-k/document-intelligence-example/refs/heads/main/data/dummy%20exercises%20-%20arm%20day%201.pdf");

Console.WriteLine($"Analyzing document {fileUri}...");

var operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, fileUri);
var analyzeResult = operation.Value;

Console.WriteLine("----------------------------------------------------");
Console.WriteLine($"Document was analyzed with model with ID: {analyzeResult.ModelId}");
Console.WriteLine($"Number of documents found: {analyzeResult.Documents.Count}");
Console.WriteLine($"Number of pages found: {analyzeResult.Pages.Count}");
Console.WriteLine("----------------------------------------------------");

// Find fields in the analyzed documents
foreach (AnalyzedDocument document in analyzeResult.Documents)
{
    document.Fields.TryGetValue("TrainingType", out var trainingTypeField);
    document.Fields.TryGetValue("Duration", out var durationField);
    
    Console.WriteLine("Training Type: " + (trainingTypeField?.Content ?? "Not found"));
    Console.WriteLine("Duration: " + (durationField?.Content ?? "Not found"));
}

// Iterate over the document tables
foreach (var table in analyzeResult.Tables)
{
    foreach (var cell in table.Cells)
    {
        if (string.IsNullOrWhiteSpace(cell.Content)) continue;
        
        switch (cell.ColumnIndex)
        {
            case 0:
                Console.WriteLine($"Phase: {cell.Content}");
                break;
            case 1:
                Console.WriteLine($"  Task: {cell.Content}");
                break;
        }
    }
}

return;

void ListAvailableModels(string endpoint, AzureKeyCredential azureKeyCredential)
{
    Console.WriteLine("Awailable models");
    var adminClient = new DocumentIntelligenceAdministrationClient(new Uri(endpoint), azureKeyCredential);
    var models = adminClient.GetModels();
    foreach (var model in models)
    {
        Console.WriteLine($"Model ID: {model.ModelId}, Model Description: {model.Description}, Created On: {model.CreatedOn}");
    }
}

