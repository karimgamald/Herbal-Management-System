using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Reviews;

public record ReviewResponse
{
    public int ReviewRecipeId { get; init; }
    public int RecipeId { get; init; }
    public float RatingValue { get; init; }
    public string? Comment { get; init; }
    public DateTime RatingDate { get; init; }
    public string HerbalistName { get; init; } = string.Empty;
}