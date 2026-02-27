using AutoMapper;
using BCrypt.Net;
using PhytoIntellect.Application.DTOs.AuthDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Services;

public class AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper) : IAuthService
{
    public async Task<AuthResultDto> RegisterAsync(RegisterUserAuthDto model, CancellationToken cancellationToken = default)
    {
        // 1. التحقق من صحة الـ Role (بناءً على طلبك)
        if (!AppRoles.IsValidRole(model.Role))
            return new AuthResultDto { Success = false, Message = $"Invalid Role. Role must be '{AppRoles.Patient}' or '{AppRoles.Herbalist}'." };

        // 2. التحقق من إن اليوزر مش موجود
        var existingUser = await unitOfWork.UserRepository.GetAsync(u => u.UserName == model.UserName, tracked: false, cancellationToken);
        if (existingUser != null)
            return new AuthResultDto { Success = false, Message = "Username already exists." };

        // 3. التحويل والتشفير
        var user = mapper.Map<User>(model);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // 4. الحفظ
        await unitOfWork.UserRepository.CreateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto { Success = true, Message = "User registered successfully." };
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto model, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(u => u.UserName == model.UserName, tracked: false, cancellationToken);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            return new AuthResultDto { Success = false, Message = "Invalid username or password." };

        var accessToken = tokenService.CreateAccessToken(user);
        var refreshToken = tokenService.CreateRefreshToken();

        var tokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = TokenHasher.HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await unitOfWork.RefreshTokenRepository.CreateAsync(tokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto { Success = true, Data = new { AccessToken = accessToken, RefreshToken = refreshToken } };
    }

    public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = TokenHasher.HashToken(refreshToken);
        var storedToken = await unitOfWork.RefreshTokenRepository.GetAsync(t => t.TokenHash == tokenHash && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow, tracked: true, cancellationToken);

        if (storedToken == null)
            return new AuthResultDto { Success = false, Message = "Invalid or expired refresh token." };

        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == storedToken.UserId, tracked: false, cancellationToken);

        storedToken.IsRevoked = true;
        unitOfWork.RefreshTokenRepository.Update(storedToken);

        var newAccessToken = tokenService.CreateAccessToken(user!);
        var newRefreshToken = tokenService.CreateRefreshToken();

        var newTokenEntity = new RefreshToken
        {
            UserId = storedToken.UserId,
            TokenHash = TokenHasher.HashToken(newRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await unitOfWork.RefreshTokenRepository.CreateAsync(newTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto { Success = true, Data = new { AccessToken = newAccessToken, RefreshToken = newRefreshToken } };
    }

    public async Task<AuthResultDto> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = TokenHasher.HashToken(refreshToken);
        var storedToken = await unitOfWork.RefreshTokenRepository.GetAsync(t => t.TokenHash == tokenHash && !t.IsRevoked, tracked: true, cancellationToken);

        if (storedToken == null)
            return new AuthResultDto { Success = false, Message = "Token not found or already revoked." };

        storedToken.IsRevoked = true;
        unitOfWork.RefreshTokenRepository.Update(storedToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto { Success = true, Message = "Logged out successfully." };
    }
}