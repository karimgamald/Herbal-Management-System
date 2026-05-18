namespace PhytoIntellect.Core.Entities;

public class Disease : LocalizedEntity
{
    public int DiseaseId { get; set; }
    public string DiseaseName { get; set; } = string.Empty;
    public string? DiseaseType { get; set; } 
    public string? Description { get; set; }
    public string? Symptoms { get; set; }
    public bool IsApproved { get; set; } = false;
    public bool IsSupportedByAi { get; set; } = false;

    // Navigation Property
    public ICollection<RecipeDisease> RecipeDiseases { get; set; } = [];
}