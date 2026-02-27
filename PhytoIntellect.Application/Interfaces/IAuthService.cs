using PhytoIntellect.Application.DTOs;
using PhytoIntellect.Application.DTOs.AuthDTOs;

namespace PhytoIntellect.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterUserAuthDto model, CancellationToken cancellationToken = default);
    Task<AuthResultDto> LoginAsync(LoginDto model, CancellationToken cancellationToken = default);
    Task<AuthResultDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<AuthResultDto> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}