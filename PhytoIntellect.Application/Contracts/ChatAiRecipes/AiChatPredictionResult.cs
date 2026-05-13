using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.ChatAiRecipes;

public class AiChatPredictionResult
{
    public string RecommendedRecipeName { get; set; } = string.Empty;
    public string MainHerb { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Preparation { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Contraindications { get; set; } = string.Empty;
    public double MatchPercentage { get; set; }
    public List<string> OtherPossibilities { get; set; } = [];
}
