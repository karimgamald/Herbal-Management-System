using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PhytoIntellect.Infrastructure.ExternalApi.ChatContracts;

public class FlaskChatAiRequest
{
    [JsonPropertyName("text")]
    public string Prompt { get; set; } = string.Empty;
}
