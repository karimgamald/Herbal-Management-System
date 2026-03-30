namespace PhytoIntellect.Core.Entities;

public class Disease
{
    public int DiseaseId { get; set; }
    public string DiseaseName { get; set; } = string.Empty;
    public string? DiseaseType { get; set; } // مثلا: مزمن، موسمي، تنفسي
    public string? Description { get; set; }
    public string? Symptoms { get; set; }
    public bool IsSupportedByAi { get; set; } = false;

    // Navigation Property
    public ICollection<RecipeDisease> RecipeDiseases { get; set; } = [];
}