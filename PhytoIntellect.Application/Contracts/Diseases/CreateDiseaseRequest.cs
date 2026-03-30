using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Diseases;

public record CreateDiseaseRequest(
    string DiseaseName,
    string? DiseaseType = null,
    string? Description = null,
    string? Symptoms = null,
    bool IsSupportedByAi = false
);