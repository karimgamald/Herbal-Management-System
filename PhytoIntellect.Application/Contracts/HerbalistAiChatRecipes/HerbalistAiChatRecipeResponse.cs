using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.HerbalistAiChatRecipes;

public class HerbalistAiChatRecipeResponse
{
    public int HerbalistId { get; set; }
    public int AiChatRecipeId { get; set; }
    public string RecommendedRecipeName { get; set; } = string.Empty;
    public string MainHerb { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}