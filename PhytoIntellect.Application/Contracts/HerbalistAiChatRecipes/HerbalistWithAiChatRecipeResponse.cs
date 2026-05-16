using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.HerbalistAiChatRecipes;

public class HerbalistWithAiChatRecipeResponse
{
    public int HerbalistId { get; set; }
    public string HerbalistName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public float AverageRating { get; set; }
}
