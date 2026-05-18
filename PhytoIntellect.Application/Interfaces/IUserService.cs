using PhytoIntellect.Application.Contracts.Accounts;
using PhytoIntellect.Application.Contracts.Users;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Interfaces;

public interface IUserService
{
    Task<PaginatedList<UserResponse>> GetAllUsersAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<UserResponse?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<string> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<string> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> UpdateAddressAsync(int userId, UpdateUserAddressRequest model, CancellationToken cancellationToken = default);
    Task<string> DeleteUserAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> UpdateMyFullNameAsync(int userId, string newFullName, CancellationToken cancellationToken = default);
    Task<string> UpdateMyUserNameAsync(int userId, string newUserName, CancellationToken cancellationToken = default);
}