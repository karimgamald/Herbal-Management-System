using AutoMapper;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using PhytoIntellect.Application.Contracts.Accounts;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Enums;

namespace PhytoIntellect.Infrastructure.Identities;

public class AuthService(
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    IMapper mapper,
    IConfiguration _config,
    IEmailService emailService) : IAuthService 
{
    public async Task<RegisterUserAuthResponse> RegisterAsync(RegisterUserAuthRequest model, 
        CancellationToken cancellationToken = default)
    {
        // 1. التحقق من الـ Role
        if (!AppRoles.IsValidRole(model.Role))
            return new RegisterUserAuthResponse
            {
                Success = false,
                Message = $"Invalid Role. Role must be '{AppRoles.Patient}' or '{AppRoles.Herbalist}'."
            };

        // 2. Validate Confirm Password
        if (model.Password != model.ConfirmPassword)
        {
            return new RegisterUserAuthResponse
            {
                Success = false,
                Message = "Password and Confirm Password do not match."
            };
        }

        // 2. التحقق من تكرار اليوزر
        var existingUser = await unitOfWork.UserRepository.GetAsync(u => u.Email == model.Email,
            tracked: false, cancellationToken: cancellationToken);

        if (existingUser != null)
            return new RegisterUserAuthResponse { Success = false, Message = "Email already exists." };

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
                User = user,
                //UserId = user.Id, // EF هيملأه تلقائياً بعد الحفظ لو مستخدم Navigation
                AverageRating = 0,
                Bio = null!,
                AvailableFrom = TimeSpan.Zero,
                AvailableTo = TimeSpan.Zero,
                //LicenseNumber = ""
                LicenseNumber = "HL-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()
            };
            
            await unitOfWork.HerbalistRepository.CreateAsync(newHerbalist, cancellationToken);
        }

        // 5. حفظ الكل (Atomic Transaction)
        // السطر ده هيبعت لـ SQL: الـ User أولاً، ياخد الـ ID، يحطه في الـ Patient، يبعت الـ Patient
        await unitOfWork.SaveChangesAsync(cancellationToken);

        //EmailMessage
        var message =
            $"""
             Welcome to Herbal System 🌿
             
             Hello {user.FullName},
             
             Your account has been successfully created.
             
             You can now login and start using the Herbal System platform.
             
             Account Email: {user.Email}
             
             If you did not create this account, please contact our support team.
             
             Best regards,
             Herbal System Team
             """;

        await emailService.SendEmailAsync(
            user.Email,
            "Welcome to Herbal System",
            message);

        return new RegisterUserAuthResponse { Success = true, Message = "User registered successfully with profile." };
    }
    public async Task<RegisterUserAuthResponse> LoginAsync(LoginAccountRequest model, 
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Email == model.Email, tracked: false,
            cancellationToken: cancellationToken);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            return new RegisterUserAuthResponse { Success = false, Message = "Invalid Email or password." };

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

        return new RegisterUserAuthResponse
        { Success = true, Data = new { AccessToken = accessToken, 
            RefreshToken = refreshToken, Role = user.Role } };
    }

    public async Task<RegisterUserAuthResponse> ResetPasswordAsync(ResetPasswordAccountRequest model,
    CancellationToken cancellationToken = default)
    {
        // 1️⃣ التأكد إن اليوزر موجود
        var user = await unitOfWork.UserRepository.GetAsync(
            u => u.Email == model.Email,
            tracked: true,
            cancellationToken: cancellationToken);

        if (user == null)
            return new RegisterUserAuthResponse
            {
                Success = false,
                Message = "User not found."
            };

        // 2️⃣ (اختياري لكن مهم جدًا) التحقق من الباسورد القديمة
        if (!string.IsNullOrWhiteSpace(model.OldPassword))
        {
            var isValidOldPassword = BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash);
            if (!isValidOldPassword)
                return new RegisterUserAuthResponse
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
        var message =
            $"""
            Password Changed Successfully
            
            Hello {user.FullName},
            
            Your password has been updated successfully.
            
            If you made this change, you can safely ignore this email.
            
            If you did NOT change your password, please contact support immediately.
            
            Best regards,
            Herbal System Security Team
            """;

        await emailService.SendEmailAsync(
            user.Email,
            "Password Changed - Herbal System",
            message);

        return new RegisterUserAuthResponse
        {
            Success = true,
            Message = "Password reset successfully."
        };
    }

    public async Task<RegisterUserAuthResponse> ForgotPasswordAsync(ForgetPasswordAccountRequest model,
    CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(
            u => u.Email == model.Email,
            tracked: true,
            cancellationToken: cancellationToken);

        if (user == null)
            return new RegisterUserAuthResponse
            {
                Success = false,
                Message = "User not found."
            };

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

        unitOfWork.UserRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var message =
           $"""
            Password Reset Notification
            
            Hello {user.FullName},
            
            Your password has been reset successfully.
            
            You can now login using your new password.
            
            If you did not request this change, please contact support immediately.
            
            Best regards,
            Herbal System Security Team
            """;

        await emailService.SendEmailAsync(
            user.Email,
            "Password Reset - Herbal System",
            message);

        return new RegisterUserAuthResponse
        {
            Success = true,
            Message = "Password reset successfully."
        };
    }

    public async Task<RegisterUserAuthResponse> RefreshTokenAsync(string refreshToken, 
        CancellationToken cancellationToken = default)
    {
        var tokenHash = TokenHasher.HashToken(refreshToken);
        var storedToken = await unitOfWork.RefreshTokenRepository.GetAsync(
            t => t.TokenHash == tokenHash && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow,
            tracked: true, cancellationToken: cancellationToken);

        if (storedToken == null)
            return new RegisterUserAuthResponse { Success = false, Message = "Invalid or expired refresh token." };

        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == storedToken.UserId, 
            tracked: false, cancellationToken: cancellationToken);

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

        return new RegisterUserAuthResponse
        { Success = true, Data = new { AccessToken = newAccessToken,
            RefreshToken = newRefreshToken } };
    }

    public async Task<RegisterUserAuthResponse> LogoutAsync(string refreshToken, 
        CancellationToken cancellationToken = default)
    {
        var tokenHash = TokenHasher.HashToken(refreshToken);
        var storedToken = await unitOfWork.RefreshTokenRepository.GetAsync(t => t.TokenHash == tokenHash && !t.IsRevoked, tracked: true, cancellationToken: cancellationToken);

        if (storedToken == null)
            return new RegisterUserAuthResponse { Success = false, Message = "Token not found or already revoked." };

        storedToken.IsRevoked = true;
        unitOfWork.RefreshTokenRepository.Update(storedToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterUserAuthResponse { Success = true, Message = "Logged out successfully." };
    }


public async Task<RegisterUserAuthResponse> GoogleLoginAsync(GoogleLoginRequest model, CancellationToken cancellationToken = default)
{
    GoogleJsonWebSignature.Payload payload;
    try
    {
        // 1. التحقق من صحة التوكن اللي جاي من جوجل
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string>() { _config["Google:ClientId"]! } // استخدمت config زي ما عملنا في الـ Primary Constructor
        };
        payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);
    }
    catch (InvalidJwtException)
    {
        return new RegisterUserAuthResponse { Success = false, Message = "Invalid Google IdToken." };
    }

    // 2. البحث عن اليوزر في الداتابيز
    var user = await unitOfWork.UserRepository.GetAsync(u => u.Email == payload.Email, tracked: false, cancellationToken: cancellationToken);

    // 3. لو اليوزر مش موجود (أول مرة يسجل بجوجل) -> هنعمله Registration
    if (user == null)
    {
        // التحقق من الـ Role المبعوت في الريكويست (لأن جوجل مش هتبعت Role، الفرونت هو اللي بيبعته مع الـ Token)
        if (string.IsNullOrEmpty(model.Role) || !AppRoles.IsValidRole(model.Role))
        {
            return new RegisterUserAuthResponse { Success = false, Message = $"Role is required for new Google accounts. Valid roles are '{AppRoles.Patient}' or '{AppRoles.Herbalist}'." };
        }

        user = new User
        {
            Email = payload.Email,
            FullName = payload.Name,
            Role = model.Role,
            // بنحط باسورد عشوائي معقد جداً مش هيستخدمه، لأن دخوله دايماً هيكون بجوجل
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString() + "Google@P@ssw0rd!"),
        };

        if (user.Role == AppRoles.Patient)
        {
            var newPatient = new Patient { User = user, Gender = Gender.Unknown };
            await unitOfWork.PatientRepository.CreateAsync(newPatient, cancellationToken);
        }
        else if (user.Role == AppRoles.Herbalist)
        {
            var newHerbalist = new Herbalist
            {
                User = user,
                AverageRating = 0,
                Bio = null!,
                AvailableFrom = TimeSpan.Zero,
                AvailableTo = TimeSpan.Zero,
                LicenseNumber = "HL-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()
            };
            await unitOfWork.HerbalistRepository.CreateAsync(newHerbalist, cancellationToken);
        }

        // إرسال إيميل الترحيب (Optional)
        var message = $"Welcome to Herbal System 🌿\n\nHello {user.FullName},\n\nYour account has been successfully created via Google Login.";
        await emailService.SendEmailAsync(user.Email, "Welcome to Herbal System", message);
    }
    // 4. لو اليوزر موجود بس مسجل رول مختلف عن اللي الفرونت باعتها (حماية إضافية)
    else if (!string.IsNullOrEmpty(model.Role) && user.Role != model.Role)
    {
        return new RegisterUserAuthResponse { Success = false, Message = "This Google account is already registered with a different role." };
    }

    await unitOfWork.SaveChangesAsync(cancellationToken);

    var accessToken = tokenService.CreateAccessToken(user);
    var refreshToken = tokenService.CreateRefreshToken();

    var refreshDuration = double.Parse(_config["JwtSettings:RefreshTokenDurationInDays"] ?? "7");

    var tokenEntity = new RefreshToken
    {
        UserId = user.Id,
        TokenHash = TokenHasher.HashToken(refreshToken),
        ExpiresAt = DateTime.UtcNow.AddDays(refreshDuration),
        IsRevoked = false
    };

    await unitOfWork.RefreshTokenRepository.CreateAsync(tokenEntity, cancellationToken);
    await unitOfWork.SaveChangesAsync(cancellationToken); // حفظ الـ Refresh Token

    return new RegisterUserAuthResponse
    {
        Success = true,
        Message = "Login successful via Google.",
        Data = new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Role = user.Role,
            FullName = user.FullName,
            ProfilePicture = payload.Picture // إضافة لطيفة للفرونت
        }
    };
}


}