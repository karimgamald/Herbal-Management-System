using System;

namespace PhytoIntellect.Core.Entities;

public class Feedback
{
    public int FeedbackId { get; set; }

    public float RatingValue { get; set; }
    public string? Comment { get; set; }
    public DateTime RatingDate { get; set; } = DateTime.UtcNow;

    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
}