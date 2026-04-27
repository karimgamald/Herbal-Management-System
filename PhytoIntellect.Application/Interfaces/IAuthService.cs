using PhytoIntellect.Application.Contracts.Accounts;

namespace PhytoIntellect.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterUserAuthResponse> RegisterAsync(RegisterUserAuthRequest model, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> ConfirmEmailAsync(string email, string token);
    Task<RegisterUserAuthResponse> LoginAsync(LoginAccountRequest model, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> GoogleLoginAsync(GoogleLoginRequest model, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> ResetPasswordAsync(ResetPasswordAccountRequest model, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> ForgotPasswordAsync(ForgetPasswordAccountRequest model, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<RegisterUserAuthResponse> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);

}