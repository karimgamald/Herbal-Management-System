using AutoMapper;
using Microsoft.Extensions.Configuration; // ضيف دي عشان الـ Configuration
using PhytoIntellect.Application.DTOs.AuthDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Enums;

namespace PhytoIntellect.Application.Services;

public class AuthService(
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    IMapper mapper,
    IConfiguration _config) : IAuthService // ضفنا الـ Configuration هنا
{
    public async Task<AuthResultDto> RegisterAsync(RegisterUserAuthDto model, 
        CancellationToken cancellationToken = default)
    {
        // 1. التحقق من الـ Role
        if (!AppRoles.IsValidRole(model.Role))
            return new AuthResultDto
            {
                Success = false,
                Message = $"Invalid Role. Role must be '{AppRoles.Patient}' or '{AppRoles.Herbalist}'."
            };

        // 2. Validate Confirm Password
        if (model.Password != model.ConfirmPassword)
        {
            return new AuthResultDto
            {
                Success = false,
                Message = "Password and Confirm Password do not match."
            };
        }

        // 2. التحقق من تكرار اليوزر
        var existingUser = await unitOfWork.UserRepository.GetAsync(u => u.Email == model.Email,
            tracked: false, cancellationToken);

        if (existingUser != null)
            return new AuthResultDto { Success = false, Message = "Email already exists." };

        // 3. تحويل الداتا وتشفير الباسورد
        var user = mapper.Map<User>(model);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // 4. الربط الذكي (Navigation Property)
        // بعد كده أنشئ البروفايل حسب الدور
        if (user.Role == AppRoles.Patient)
        {
            var newPatient = new Patient
            {
                User = user,
                BirthDate = null,
                Gender = Gender.Unknown
            };

            await unitOfWork.PatientRepository.CreateAsync(newPatient, cancellationToken);
        }

        else if (user.Role == AppRoles.Herbalist)
        {
            var newHerbalist = new Herbalist
            {
                User = user
            };

            await unitOfWork.HerbalistRepository.CreateAsync(newHerbalist, cancellationToken);
        }

        // 5. حفظ الكل (Atomic Transaction)
        // السطر ده هيبعت لـ SQL: الـ User أولاً، ياخد الـ ID، يحطه في الـ Patient، يبعت الـ Patient
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto { Success = true, Message = "User registered successfully with profile." };
    }
    public async Task<AuthResultDto> LoginAsync(LoginRequestDto model, 
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Email == model.Email, tracked: false, 
            cancellationToken);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            return new AuthResultDto { Success = false, Message = "Invalid username or password." };

        var accessToken = tokenService.CreateAccessToken(user);
        var refreshToken = tokenService.CreateRefreshToken();

        // 🕒 سحب مدة الـ Refresh Token من الإعدادات (لو مش موجودة هنخليها 7 أيام افتراضياً)
        var refreshDuration = double.Parse(_config["JwtSettings:RefreshTokenDurationInDays"] ?? "7");

        var tokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = TokenHasher.HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDuration), // مدة ديناميكية
            IsRevoked = false
        };

        await unitOfWork.RefreshTokenRepository.CreateAsync(tokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto { Success = true, Data = new { AccessToken = accessToken, 
            RefreshToken = refreshToken } };
    }

    public async Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto model,
    CancellationToken cancellationToken = default)
    {
        // 1️⃣ التأكد إن اليوزر موجود
        var user = await unitOfWork.UserRepository.GetAsync(
            u => u.Email == model.Email,
            tracked: true,
            cancellationToken);

        if (user == null)
            return new AuthResultDto
            {
                Success = false,
                Message = "User not found."
            };

        // 2️⃣ (اختياري لكن مهم جدًا) التحقق من الباسورد القديمة
        if (!string.IsNullOrWhiteSpace(model.OldPassword))
        {
            var isValidOldPassword = BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash);
            if (!isValidOldPassword)
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Old password is incorrect."
                };
        }

        // 3️⃣ تشفير الباسورد الجديدة
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

        // 4️⃣ تحديث اليوزر
        unitOfWork.UserRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto
        {
            Success = true,
            Message = "Password reset successfully."
        };
    }

    public async Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordDto model,
    CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(
            u => u.Email == model.Email,
            tracked: true,
            cancellationToken);

        if (user == null)
            return new AuthResultDto
            {
                Success = false,
                Message = "User not found."
            };

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

        unitOfWork.UserRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto
        {
            Success = true,
            Message = "Password reset successfully."
        };
    }

    public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken, 
        CancellationToken cancellationToken = default)
    {
        var tokenHash = TokenHasher.HashToken(refreshToken);
        var storedToken = await unitOfWork.RefreshTokenRepository.GetAsync(
            t => t.TokenHash == tokenHash && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow,
            tracked: true, cancellationToken);

        if (storedToken == null)
            return new AuthResultDto { Success = false, Message = "Invalid or expired refresh token." };

        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == storedToken.UserId, 
            tracked: false, cancellationToken);

        // إلغاء التوكن القديم (Rotation)
        storedToken.IsRevoked = true;
        unitOfWork.RefreshTokenRepository.Update(storedToken);

        var newAccessToken = tokenService.CreateAccessToken(user!);
        var newRefreshToken = tokenService.CreateRefreshToken();

        var refreshDuration = double.Parse(_config["JwtSettings:RefreshTokenDurationInDays"] ?? "7");

        var newTokenEntity = new RefreshToken
        {
            UserId = storedToken.UserId,
            TokenHash = TokenHasher.HashToken(newRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDuration), // مدة ديناميكية
            IsRevoked = false
        };

        await unitOfWork.RefreshTokenRepository.CreateAsync(newTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto { Success = true, Data = new { AccessToken = newAccessToken,
            RefreshToken = newRefreshToken } };
    }

    public async Task<AuthResultDto> LogoutAsync(string refreshToken, 
        CancellationToken cancellationToken = default)
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