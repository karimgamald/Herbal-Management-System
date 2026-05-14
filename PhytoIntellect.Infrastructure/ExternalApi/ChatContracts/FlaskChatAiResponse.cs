using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PhytoIntellect.Infrastructure.ExternalApi.ChatContracts;

public class FlaskChatAiResponse
{
    [JsonPropertyName("predictions")]
    public List<FlaskPredictionItem> Predictions { get; set; } = [];
}

public class FlaskPredictionItem
{
    [JsonPropertyName("recipe")] 
    public string RecipeName { get; set; } = string.Empty;

    [JsonPropertyName("confidence")] 
    public double Confidence { get; set; }

    [JsonPropertyName("main_herb")]
    public string MainHerb { get; set; } = string.Empty;

    [JsonPropertyName("scientific_name")]
    public string ScientificName { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("preparation")]
    public string Preparation { get; set; } = string.Empty;

    [JsonPropertyName("dosage")]
    public string Dosage { get; set; } = string.Empty;

    [JsonPropertyName("contraindications")]
    public string Contraindications { get; set; } = string.Empty;
}