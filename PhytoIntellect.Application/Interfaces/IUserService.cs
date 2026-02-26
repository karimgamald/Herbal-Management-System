using PhytoIntellect.Application.DTOs.PhytoIntellect.Application.DTOs;
using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Interfaces
{
    public interface IUserService
    {
        // ---------- User Validation ----------
        Task<User?> ValidateUserAsync(string username, string password);
        Task<User?> ValidateByUserNameAsync(string username);

        // ---------- CRUD ----------
        Task<IEnumerable<User?>> GetAllUsersAsync();
        Task<User?> AddUserAsync(User user);
        Task<User?> UpdateUserAsync(User user);
        Task<User?> DeleteUserAsync(User user);

        // ---------- Password & Refresh Token ----------
        Task<AuthResultDTO> ResetPasswordAsync(ResetPasswordDTO model);
        Task AddRefreshTokenAsync(int userId, string refreshToken);
        Task<AuthResultDTO> RefreshTokenAsync(string refreshToken, ITokenService tokenService);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    }
}