using PhytoIntellect.Application.Contracts.Accounts;
using PhytoIntellect.Application.DTOs;

namespace PhytoIntellect.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterUserAuthResponse> RegisterAsync(RegisterUserAuthRequest model, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> LoginAsync(LoginAccountRequest model, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> ResetPasswordAsync(ResetPasswordAccountRequest model, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> ForgotPasswordAsync(ForgetPasswordAccountRequest model, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}