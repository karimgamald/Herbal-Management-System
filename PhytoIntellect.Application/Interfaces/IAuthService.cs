using PhytoIntellect.Application.DTOs;
using PhytoIntellect.Application.DTOs.AuthDTOs;

namespace PhytoIntellect.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterUserAuthDto model, CancellationToken cancellationToken = default);
    Task<AuthResultDto> LoginAsync(LoginRequestDto model, CancellationToken cancellationToken = default);
    Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto model, CancellationToken cancellationToken = default);
    Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordDto model, CancellationToken cancellationToken = default);
    Task<AuthResultDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<AuthResultDto> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}