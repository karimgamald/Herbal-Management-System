namespace PhytoIntellect.Core.Entities;

public class RecipeDisease
{
    public int RecipeDiseaseId { get; set; }
    public int RecipeId { get; set; }
    public int DiseaseId { get; set; }

    public Recipe Recipe { get; set; } = null!;
    public Disease Disease { get; set; } = null!;
}