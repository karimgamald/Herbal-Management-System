using PhytoIntellect.Application.DTOs.HerbalistDTOs;

namespace PhytoIntellect.Application.Interfaces;

public interface IHerbalistService
{
    Task<HerbalistDto?> GetMyProfileAsync(int userId, CancellationToken cancellationToken);

    Task<string> CreateProfileAsync(int userId, CreateOrUpdateHerbalistDto request, CancellationToken cancellationToken);

    Task<string> UpdateMyProfileAsync(int userId, CreateOrUpdateHerbalistDto request, CancellationToken cancellationToken);

    Task<HerbalistDto?> GetHerbalistByIdAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<HerbalistDto>> GetAllHerbalistsAsync(CancellationToken cancellationToken);
}