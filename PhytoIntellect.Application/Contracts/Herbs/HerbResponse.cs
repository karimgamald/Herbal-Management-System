namespace PhytoIntellect.Application.Contracts.Herbs;

public class HerbResponse
{
    public int HerbId { get; set; }

    public string HerbName { get; set; } = string.Empty;

    public string? ScientificName { get; set; }

    public string? Description { get; set; }

    public string? Benefits { get; set; }

    public string? Dosage { get; set; }

    public string? Warnings { get; set; }

    public string? ImageURL { get; set; }

    public bool IsApproved { get; set; }
}