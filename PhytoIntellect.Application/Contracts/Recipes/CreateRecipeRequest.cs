using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Recipes;

public record CreateRecipeRequest
{
    public string Description { get; init; } = string.Empty;
    public string Instructions { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public List<RecipeHerbRequest> Herbs { get; init; } = [];
    public List<int> DiseaseIds { get; init; } = [];
}