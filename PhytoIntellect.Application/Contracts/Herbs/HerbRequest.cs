using Microsoft.AspNetCore.Http;

namespace PhytoIntellect.Application.Contracts.Herbs;

public record HerbRequest
{
    public string HerbName { get; set; } = string.Empty;

    public string? ScientificName { get; set; }

    public string? Description { get; set; }

    public string? Benefits { get; set; }

    public string? Dosage { get; set; }

    public string? Warnings { get; set; }

    public IFormFile? Image { get; set; }
}