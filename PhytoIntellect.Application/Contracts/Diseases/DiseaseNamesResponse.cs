using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Diseases;

public record DiseaseNamesResponse(
    int DiseaseId, 
    string DiseaseName
);
