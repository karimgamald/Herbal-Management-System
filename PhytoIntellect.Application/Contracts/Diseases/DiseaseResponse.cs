using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Diseases;

public record DiseaseResponse
{
    public int DiseaseId { get; init; }
    public string DiseaseName { get; init; } = string.Empty;
    public string? DiseaseType { get; init; }
    public string? Description { get; init; }
    public string? Symptoms { get; init; }
}