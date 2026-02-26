using PhytoIntellect.Api.DTOs.UserDTOs;
using PhytoIntellect.Application.DTOs;
using PhytoIntellect.Application.DTOs.PhytoIntellect.Application.DTOs;
using PhytoIntellect.Application.DTOs.UserDTOs;

namespace PhytoIntellect.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDTO> RegisterAsync(RegisterUserDTO model);
        Task<AuthResultDTO> LoginAsync(UserDTO model);
        Task<AuthResultDTO> RefreshAsync(string refreshToken);
        Task<AuthResultDTO> LogoutAsync(string refreshToken);
    }
}