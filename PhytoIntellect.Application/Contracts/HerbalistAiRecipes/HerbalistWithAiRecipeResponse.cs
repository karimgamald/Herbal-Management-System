using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.HerbalistAiRecipes;

public class HerbalistWithAiRecipeResponse
{
    public int HerbalistId { get; set; }
    public string HerbalistName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
