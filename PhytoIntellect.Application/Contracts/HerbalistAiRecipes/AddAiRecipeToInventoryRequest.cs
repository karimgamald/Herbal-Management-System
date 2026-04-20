using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.HerbalistAiRecipes;

public class AddAiRecipeToInventoryRequest
{
    public int AiRecipeId { get; set; }
    public decimal Price { get; set; }
}