using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Recipes;

public record UpdateRecipeRequest
{
    public string Description { get; init; } = string.Empty;
    public string Instructions { get; init; } = string.Empty;
    public List<RecipeHerbRequest> Herbs { get; init; } = new();
}