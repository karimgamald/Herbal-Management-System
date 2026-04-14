using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.AiRecipes;

public class AiRecipeResponse
{
    public int RecipeId { get; set; } 
    public string RecommendedRecipeName { get; set; }
    public string Condition { get; set; }
    public double ConfidenceScore { get; set; }
    public List<string> PreparationInstructions { get; set; }
    public string CautionWarning { get; set; }
}