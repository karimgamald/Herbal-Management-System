using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Recipes;

public record RecipeHerbResponse
{
    public int HerbId { get; init; }
    public string HerbName { get; init; } = string.Empty;
    public float Quantity { get; init; }
}