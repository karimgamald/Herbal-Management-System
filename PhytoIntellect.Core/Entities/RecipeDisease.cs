namespace PhytoIntellect.Core.Entities;

public class RecipeDisease
{
    public int RecipeDiseaseId { get; set; } // الـ Primary Key بتاعك زي ما إنت كاتب في الاسكيما
    public int RecipeId { get; set; }
    public int DiseaseId { get; set; }

    public Recipe Recipe { get; set; } = null!;
    public Disease Disease { get; set; } = null!;
}