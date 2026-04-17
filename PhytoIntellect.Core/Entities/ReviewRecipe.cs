

namespace PhytoIntellect.Core.Entities;

public class ReviewRecipe
{
    public int ReviewRecipeId { get; set; }

    public float RatingValue { get; set; }
    public string? Comment { get; set; }
    public DateTime RatingDate { get; set; } = DateTime.UtcNow;

    public int AiRecipeId { get; set; }
    public AiRecipe? AiRecipe { get; set; }

    public int HerbalistId { get; set; }
    public Herbalist? Herbalist { get; set; }
} 