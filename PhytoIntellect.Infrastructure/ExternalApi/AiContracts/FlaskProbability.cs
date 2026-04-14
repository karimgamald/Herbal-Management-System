using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PhytoIntellect.Infrastructure.ExternalApi.AiContracts;

public class FlaskProbability
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("probability")]
    public double Probability { get; set; }
}