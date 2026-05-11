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
        if (!AppRoles.IsValidRole(model.Role))
            return new RegisterUserAuthResponse
            {
                Success = false,
                Message = $"Invalid Role. Role must be '{AppRoles.Patient}' or '{AppRoles.Herbalist}'."
            };

        if (model.Password != model.ConfirmPassword)
        {
            return new RegisterUserAuthResponse
            {
                Success = false,
                Message = "Password and Confirm Password do not match."
            };
        }

        var existingUser = await unitOfWork.UserRepository.GetAsync(u => u.Email == model.Email,
            tracked: false, cancellationToken: cancellationToken);

        if (existingUser != null)
            return new RegisterUserAuthResponse { Success = false, Message = "Email already exists." };

        var user = mapper.Map<User>(model);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        var token = Guid.NewGuid().ToString("N");

        user.EmailConfirmationToken = token;
        user.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24);
        user.IsEmailConfirmed = false;

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
                AverageRating = 0,
                Bio = null!,
                AvailableFrom = TimeSpan.Zero,
                AvailableTo = TimeSpan.Zero,
                LicenseNumber = "HL-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()
            };
            
            await unitOfWork.HerbalistRepository.CreateAsync(newHerbalist, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var baseUrl = _config["App:BaseUrl"];

        var confirmationLink =
            $"{baseUrl}/api/accounts/confirm-email" +
            $"?email={Uri.EscapeDataString(user.Email)}" +
            $"&token={Uri.EscapeDataString(token)}";

        var message = $@"
        <html>
         <body style='font-family: Arial; text-align: center;'>
         
             <h2>Welcome to Herbal System 🌿</h2>
         
             <p>Hello {user.FullName},</p>
         
             <p>Please confirm your email by clicking the button below:</p>
         
             <a href='{confirmationLink}' 
                style='display:inline-block;
                       padding:12px 25px;
                       background-color:#28a745;
                       color:white;
                       text-decoration:none;
                       border-radius:5px;
                       font-size:16px;'>
                 Confirm Email
             </a>
         
             <p style='margin-top:20px;'>This link expires in 24 hours.</p>
         
         </body>
        </html>
         ";

        await emailService.SendEmailAsync(user.Email, "Confirm Your Email", message);

        return new RegisterUserAuthResponse
        {
            Success = true,
            Message = $"User registered successfully. Please confirm your email."

        };
    }

    public async Task<RegisterUserAuthResponse> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        // 1. Fail-Fast Validation
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            return new RegisterUserAuthResponse { Success = false, Message = "Email and token are required." };

        // 2. Fetch User
        var user = await unitOfWork.UserRepository.GetAsync(
            u => u.Email == email,
            tracked: true,
            cancellationToken: cancellationToken);

        if (user == null)
            return new RegisterUserAuthResponse { Success = false, Message = "User not found." };

        // 3. Check if already confirmed
        if (user.IsEmailConfirmed)
            return new RegisterUserAuthResponse { Success = true, Message = "Email already confirmed." };

        // 4. Validate Token & Expiry
        if (user.EmailConfirmationToken != token || user.EmailConfirmationTokenExpiry < DateTime.UtcNow)
        {
            return new RegisterUserAuthResponse { Success = false, Message = "Invalid or expired token." };
        }

        // 5. Update User
        user.IsEmailConfirmed = true;
        user.EmailConfirmationToken = null;
        user.EmailConfirmationTokenExpiry = null;

        unitOfWork.UserRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterUserAuthResponse { Success = true, Message = "Email confirmed successfully." };
    }

    public async Task<RegisterUserAuthResponse> ResendConfirmationEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // 1. نتأكد إن اليوزر موجود
        var user = await unitOfWork.UserRepository.GetAsync(
            u => u.Email == email,
            tracked: true, // لازم true عشان هنعدل عليه
            cancellationToken: cancellationToken);

        if (user == null)
            return new RegisterUserAuthResponse { Success = false, Message = "User not found." };

        // 2. لو اليوزر متفعل أصلاً، مفيش داعي نبعتله إيميل تاني
        if (user.IsEmailConfirmed)
            return new RegisterUserAuthResponse { Success = false, Message = "Account is already confirmed. You can login directly." };

        // 3. نكريت توكن جديد بمدة جديدة (24 ساعة)
        var newToken = Guid.NewGuid().ToString("N");
        user.EmailConfirmationToken = newToken;
        user.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24);

        unitOfWork.UserRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. نجهز اللينك ونبعت الإيميل أبو زرار أخضر
        var confirmationLink = $"{_config["App:BaseUrl"]}/api/accounts/confirm-email?email={user.Email}&token={newToken}";

        var message = $@"
    <html>
        <body style='font-family: Arial; text-align: center;'>
            <h2>Welcome Back to Herbal System 🌿</h2>
            <p>Hello {user.FullName},</p>
            <p>You requested a new confirmation link. Please verify your email by clicking the button below:</p>
            <a href='{confirmationLink}' 
               style='display:inline-block; padding:12px 25px; background-color:#28a745; color:white; text-decoration:none; border-radius:5px; font-size:16px;'>
                Verify Email
            </a>
            <p style='margin-top:20px;'>This link is valid for 24 hours.</p>
        </body>
    </html>";

        await emailService.SendEmailAsync(user.Email, "New Confirmation Link - Herbal System", message);

        return new RegisterUserAuthResponse
        {
            Success = true,
            Message = "A new confirmation email has been sent successfully. Please check your inbox."
        };
    }

    public async Task<RegisterUserAuthResponse> LoginAsync(LoginAccountRequest model, 
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Email == model.Email, tracked: false,
            cancellationToken: cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            return new RegisterUserAuthResponse { Success = false, Message = "Invalid Email or password." };

        // ❗ منع الدخول قبل التفعيل
        //if (!user.IsEmailConfirmed)
        //    return new RegisterUserAuthResponse
        //    {
        //        Success = false,
        //        Message = "Please confirm your email before logging in."
        //    };

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

    public async Task<RegisterUserAuthResponse> GoogleLoginAsync(GoogleLoginRequest model,
        CancellationToken cancellationToken = default)
    {
        GoogleJsonWebSignature.Payload payload;

        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _config["Google:ClientId"]! }
            };

            payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);
        }
        catch
        {
            return new RegisterUserAuthResponse
            { Success = false, Message = "Invalid Google IdToken." };
        }

        var user = await unitOfWork.UserRepository.GetAsync(
            u => u.Email == payload.Email,
            tracked: false,
            cancellationToken: cancellationToken);

        if (user == null)
        {
            if (string.IsNullOrEmpty(model.Role) || !AppRoles.IsValidRole(model.Role))
                return new RegisterUserAuthResponse
                {
                    Success = false,
                    Message = "Role is required for new Google accounts, Valid roles are Patient or Herbalist"
                };

            user = new User
            {
                Email = payload.Email,
                FullName = payload.Name,
                Role = model.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),

                // ✅ Google users are auto-confirmed
                IsEmailConfirmed = true
            };

            if (user.Role == AppRoles.Patient)
            {
                await unitOfWork.PatientRepository.CreateAsync(
                    new Patient { User = user, Gender = Gender.Unknown },
                    cancellationToken);
            }
            else
            {
                await unitOfWork.HerbalistRepository.CreateAsync(
                    new Herbalist
                    {
                        User = user,
                        LicenseNumber = "HL-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()
                    },
                    cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var accessToken = tokenService.CreateAccessToken(user);
        var refreshToken = tokenService.CreateRefreshToken();

        var refreshDuration = double.Parse(_config["JwtSettings:RefreshTokenDurationInDays"] ?? "7");

        await unitOfWork.RefreshTokenRepository.CreateAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = TokenHasher.HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDuration)
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterUserAuthResponse
        {
            Success = true,
            Message = "Login successful via Google.",
            Data = new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Role = user.Role
            }
        };
    }

   public async Task<RegisterUserAuthResponse> ResetPasswordAsync(ResetPasswordAccountRequest model,
    CancellationToken cancellationToken = default)
    {
        // 1️⃣ Get user
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

        // =========================================
        // 🔐 CASE 1: Reset using TOKEN (Forgot Password Flow)
        // =========================================
        if (!string.IsNullOrEmpty(model.Token))
        {
            if (user.PasswordResetToken == null ||
                user.PasswordResetTokenExpiry == null ||
                user.PasswordResetToken != model.Token ||
                user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return new RegisterUserAuthResponse
                {
                    Success = false,
                    Message = "Invalid or expired reset token."
                };
            }

            // update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            // clear token after use
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
        }

        // =========================================
        // 🔐 CASE 2: Reset using OLD PASSWORD (Logged-in user)
        // =========================================
        else
        {
            if (string.IsNullOrWhiteSpace(model.OldPassword))
            {
                return new RegisterUserAuthResponse
                {
                    Success = false,
                    Message = "Old password or reset token is required."
                };
            }

            var isValidOldPassword = BCrypt.Net.BCrypt.Verify(
                model.OldPassword,
                user.PasswordHash);

            if (!isValidOldPassword)
            {
                return new RegisterUserAuthResponse
                {
                    Success = false,
                    Message = "Old password is incorrect."
                };
            }

            // update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        }

        // 3️⃣ Save changes
        unitOfWork.UserRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 4️⃣ Send notification email
        var message = $"""
    Password Changed Successfully
    
    Hello {user.FullName},
    
    Your password has been updated successfully.
    
    If this wasn't you, please contact support immediately.
    
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

    public async Task<RegisterUserAuthResponse> ForgotPasswordAsync(ForgetPasswordAccountRequest model, CancellationToken cancellationToken = default)
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

        // ❌ invalidate old token (important)
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        // 🔐 generate new token
        var token = Guid.NewGuid().ToString("N");

        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        unitOfWork.UserRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var baseUrl = _config["App:BaseUrl"];

        var resetLink =
     $"{_config["App:BaseUrl"]}/api/accounts/reset-password" +
     $"?email={Uri.EscapeDataString(user.Email)}" +
     $"&token={Uri.EscapeDataString(token)}";

        var message = $@"
    <html>
     <body style='font-family: Arial; text-align: center;'>
     
         <h2>Reset Your Password 🔐</h2>
     
         <p>Hello {user.FullName},</p>
     
         <p>You requested to reset your password.</p>
     
         <p>Click the button below:</p>
     
         <a href='{resetLink}' 
            style='display:inline-block;
                   padding:12px 25px;
                   background-color:#dc3545;
                   color:white;
                   text-decoration:none;
                   border-radius:5px;
                   font-size:16px;'>
             Reset Password
         </a>
     
         <p style='margin-top:20px;'>This link expires in 1 hour.</p>
     
     </body>
    </html>
    ";

        await emailService.SendEmailAsync(user.Email, "Reset Your Password", message);

        return new RegisterUserAuthResponse
        {
            Success = true,
            Message = "Password reset link sent to your email."
        };
    }

    public async Task<RegisterUserAuthResponse> ValidateResetTokenAsync(string email, string token)
    {
        // 1️⃣ Basic validation
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return new RegisterUserAuthResponse
            {
                Success = false,
                Message = "Email and token are required."
            };
        }

        // 2️⃣ Get user
        var user = await unitOfWork.UserRepository.GetAsync(
            u => u.Email == email,
            tracked: false);

        if (user == null)
        {
            return new RegisterUserAuthResponse
            {
                Success = false,
                Message = "User not found."
            };
        }

        // 3️⃣ Check if token exists
        if (string.IsNullOrWhiteSpace(user.PasswordResetToken) ||
            user.PasswordResetTokenExpiry == null)
        {
            return new RegisterUserAuthResponse
            {
                Success = false,
                Message = "Invalid reset request."
            };
        }

        // 4️⃣ Validate token match
        if (user.PasswordResetToken != token)
        {
            return new RegisterUserAuthResponse
            {
                Success = false,
                Message = "Invalid token."
            };
        }

        // 5️⃣ Check expiry
        if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            return new RegisterUserAuthResponse
            {
                Success = false,
                Message = "Token expired. Please request a new password reset."
            };
        }

        // 6️⃣ Success
        return new RegisterUserAuthResponse
        {
            Success = true,
            Message = "Token is valid."
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
}