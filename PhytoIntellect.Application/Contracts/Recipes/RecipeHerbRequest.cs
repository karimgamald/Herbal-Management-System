using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Recipes;

public record RecipeHerbRequest
{
    public int HerbId { get; init; }
    public float Quantity { get; init; }
}