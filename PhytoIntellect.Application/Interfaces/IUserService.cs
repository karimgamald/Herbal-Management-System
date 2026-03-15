using PhytoIntellect.Application.DTOs.AuthDTOs;
using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<string> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken = default);
    Task<string> UpdateUserAsync(int id, UpdateUserDto request, CancellationToken cancellationToken = default);
    Task<AuthResultDto> UpdateAddressAsync(int userId, UpdateUserAddressDto model, CancellationToken cancellationToken = default);
    Task<string> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
}