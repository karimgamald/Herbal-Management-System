using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Feedbacks;

public record FeedbackResponse
{
    public int FeedbackId { get; init; }

    public int? RecipeId { get; init; }
    public int? AiRecipeId { get; init; }

    public float RatingValue { get; init; }
    public string? Comment { get; init; }
    public DateTime RatingDate { get; init; }
    public string PatientName { get; init; } = string.Empty;
}
