using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Recipes;

public record RecipeResponse
{
    public int RecipeId { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Instructions { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }

    public bool IsActive { get; init; }
    public decimal Price { get; init; }
    public float AverageRating { get; init; }
    public int TotalRatings { get; init; }

    public List<RecipeHerbResponse> Herbs { get; init; } = new();
    public List<RecipeDiseaseResponse> TargetedDiseases { get; init; } = new();
}