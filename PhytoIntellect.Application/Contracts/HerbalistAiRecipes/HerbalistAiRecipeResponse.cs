using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.HerbalistAiRecipes;

public class HerbalistAiRecipeResponse
{
    public int AiRecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public int HerbalistId { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}