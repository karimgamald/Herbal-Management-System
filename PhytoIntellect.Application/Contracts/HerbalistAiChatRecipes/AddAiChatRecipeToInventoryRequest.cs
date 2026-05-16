using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.HerbalistAiChatRecipes;

public class AddAiChatRecipeToInventoryRequest
{
    public int AiChatRecipeId { get; set; }
    public decimal Price { get; set; }
}