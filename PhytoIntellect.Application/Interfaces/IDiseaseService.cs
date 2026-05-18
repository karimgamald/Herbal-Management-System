using PhytoIntellect.Application.Contracts.Diseases;
using PhytoIntellect.Application.Paginations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IDiseaseService
{
    Task<PaginatedList<DiseaseResponse>> GetAllDiseasesAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<IEnumerable<DiseaseNamesResponse>> GetDiseasesNameAsync(CancellationToken cancellationToken = default);
    Task<DiseaseResponse> CreateDiseaseAsync(CreateDiseaseRequest request, CancellationToken cancellationToken = default);

    // Admin Features
    Task<PaginatedList<DiseaseResponse>> GetPendingDiseasesAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<DiseaseResponse> AddDiseaseByAdminAsync(CreateDiseaseRequest request, bool isAiSupported, CancellationToken cancellationToken = default);
    Task<bool> ApproveDiseaseAsync(int diseaseId, CancellationToken cancellationToken = default);
    Task<bool> RejectDiseaseAsync(int diseaseId, CancellationToken cancellationToken = default);
}
