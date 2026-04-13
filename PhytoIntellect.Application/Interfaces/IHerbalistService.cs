using PhytoIntellect.Application.Contracts.Herbalists;

namespace PhytoIntellect.Application.Interfaces;

public interface IHerbalistService
{
    Task<HerbalistResponse?> GetMyProfileAsync(int userId, CancellationToken cancellationToken);

    Task<string> CreateProfileAsync(int userId, CreateOrUpdateHerbalistRequest request, CancellationToken cancellationToken);

    Task<string> UpdateMyProfileAsync(int userId, CreateOrUpdateHerbalistRequest request, CancellationToken cancellationToken);

    Task<HerbalistResponse?> GetHerbalistByIdAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<HerbalistResponse>> GetAllHerbalistsAsync(CancellationToken cancellationToken);
}