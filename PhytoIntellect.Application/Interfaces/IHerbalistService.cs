using PhytoIntellect.Application.Contracts.Herbalists;
using PhytoIntellect.Application.Paginations;

namespace PhytoIntellect.Application.Interfaces;

public interface IHerbalistService
{
    Task<HerbalistResponse?> GetMyProfileAsync(int userId, CancellationToken cancellationToken);
    Task<string> CreateProfileAsync(int userId, CreateOrUpdateHerbalistRequest request, CancellationToken cancellationToken);
    Task<string> UpdateMyProfileAsync(int userId, CreateOrUpdateHerbalistRequest request, CancellationToken cancellationToken);
    Task<HerbalistResponse?> GetHerbalistByIdAsync(int id, CancellationToken cancellationToken);
    Task<PaginatedList<HerbalistResponse>> GetAllHerbalistsAsync(RequestFilters filters, CancellationToken cancellationToken = default);

    Task<PaginatedList<HerbalistResponse>> AdminGetAllHerbalistsAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<bool> DeleteHerbalistAsync(int herbalistId, CancellationToken cancellationToken = default);
    Task<object> GetHerbalistsStatsAsync(CancellationToken cancellationToken = default);
}