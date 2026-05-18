using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.HerbalistHerb;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Application.Services;

public interface IHerbService
{
    Task<PaginatedList<HerbResponse>> GetApprovedHerbsAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<PaginatedList<HerbResponse>> GetPendingHerbsAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<HerbResponse?> GetHerbByIdAsync(int herbId, CancellationToken cancellationToken = default);
    Task<HerbWithHerbalistResponse?> GetHerbWithHerbalistAsync(int herbId, CancellationToken cancellationToken = default);
    Task<IEnumerable<HerbalistHerbResponse>> GetHerbalistsByHerbIdAsync(int herbId, CancellationToken cancellationToken = default);

    Task<HerbResponse?> CreateHerbAsync(int userId, HerbRequest request, CancellationToken cancellationToken = default);
    Task<HerbResponse?> UpdateHerbAsync(int userId, int herbId, HerbRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteHerbAsync(int userId, int herbId, CancellationToken cancellationToken = default);

    Task<HerbResponse> AdminCreateHerbAsync(HerbRequest request, CancellationToken cancellationToken = default);
    Task<HerbResponse?> AdminUpdateHerbAsync(int herbId, HerbRequest request, CancellationToken cancellationToken = default);
    Task<bool> AdminDeleteHerbAsync(int herbId, CancellationToken cancellationToken = default);
    Task<bool> ApproveHerbAsync(int herbId, CancellationToken cancellationToken = default);
}



