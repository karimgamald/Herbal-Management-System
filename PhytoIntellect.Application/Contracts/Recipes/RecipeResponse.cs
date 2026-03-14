using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Recipes;

public record RecipeResponse
{
    public int RecipeId { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Instructions { get; init; } = string.Empty;
    public bool CreatedByAI { get; init; }
    public DateTime CreatedDate { get; init; }
    public List<RecipeHerbResponse> Herbs { get; init; } = new();
    public List<RecipeDiseaseResponse> TargetedDiseases { get; init; } = new();
}