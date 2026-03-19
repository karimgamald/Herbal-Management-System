using PhytoIntellect.Application.Contracts.Diseases;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IDiseaseService
{
    Task<IEnumerable<DiseaseResponse>> GetAllDiseasesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DiseaseNamesResponse>> GetDiseasesNameAsync(CancellationToken cancellationToken = default);
    Task<DiseaseResponse> CreateDiseaseAsync(CreateDiseaseRequest request, CancellationToken cancellationToken = default);
}