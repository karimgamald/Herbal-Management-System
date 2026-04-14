using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PhytoIntellect.Infrastructure.ExternalApi.AiContracts;

public class FlaskRecipeDetails
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("condition")]
    public string Condition { get; set; }

    [JsonPropertyName("instructions")]
    public List<string> Instructions { get; set; }

    [JsonPropertyName("caution")]
    public string Caution { get; set; }

    [JsonPropertyName("emoji")]
    public string Emoji { get; set; }
}