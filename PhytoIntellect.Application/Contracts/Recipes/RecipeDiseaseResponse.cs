using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Recipes;

public record RecipeDiseaseResponse
{
    public int DiseaseId { get; init; }
    public string DiseaseName { get; init; } = string.Empty;
}