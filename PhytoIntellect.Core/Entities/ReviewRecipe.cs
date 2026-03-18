

namespace PhytoIntellect.Core.Entities;

public class ReviewRecipe
{
    public int ReviewRecipeId { get; set; }

    public float RatingValue { get; set; }
    public string? Comment { get; set; }
    public DateTime RatingDate { get; set; } = DateTime.UtcNow;

    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    public int HerbalistId { get; set; }
    public Herbalist? Herbalist { get; set; }
}