using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PhytoIntellect.Infrastructure.ExternalApi.AiContracts;

public class FlaskAiResponse
{
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    [JsonPropertyName("confident")]
    public bool IsConfident { get; set; }

    [JsonPropertyName("recipe")]
    public FlaskRecipeDetails Recipe { get; set; }

    [JsonPropertyName("all_probabilities")]
    public List<FlaskProbability> AllProbabilities { get; set; }
}