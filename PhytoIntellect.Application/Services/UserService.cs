using PhytoIntellect.Application.DTOs.PhytoIntellect.Application.DTOs;
using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // --------------------- VALIDATE USER ---------------------
        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == username);

            if (user == null)
                return null;

            // Verify hashed password
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<User?> ValidateByUserNameAsync(string username)
        {
            return await _unitOfWork.UserRepository.GetAsync(u => u.UserName == username);
        }

        // --------------------- CRUD ---------------------
        public async Task<IEnumerable<User?>> GetAllUsersAsync()
        {
            return await _unitOfWork.UserRepository.GetAllAsync();
        }

        public async Task<User?> AddUserAsync(User user)
        {
            await _unitOfWork.UserRepository.CreateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUserAsync(User user)
        {
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }

        public async Task<User?> DeleteUserAsync(User user)
        {
            await _unitOfWork.UserRepository.RemoveAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }

        // --------------------- RESET PASSWORD ---------------------
        public async Task<AuthResultDTO> ResetPasswordAsync(ResetPasswordDTO model)
        {
            var user = await ValidateByUserNameAsync(model.UserName);
            if (user == null)
                return new AuthResultDTO { Success = false, Message = "User not found." };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResultDTO { Success = true, Message = "Password reset successfully." };
        }

        // --------------------- REFRESH TOKEN MANAGEMENT ---------------------

        public async Task AddRefreshTokenAsync(int userId, string refreshToken)
        {
            var tokenEntity = new RefreshToken
            {
                UserId = userId,
                TokenHash = TokenHasher.HashToken(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokenRepository.CreateAsync(tokenEntity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<AuthResultDTO> RefreshTokenAsync(string refreshToken, ITokenService tokenService)
        {
            var tokenHash = TokenHasher.HashToken(refreshToken);

            var storedToken = await _unitOfWork.RefreshTokenRepository.GetAsync(
                t => t.TokenHash == tokenHash && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow
            );

            if (storedToken == null)
                return new AuthResultDTO { Success = false, Message = "Invalid or expired refresh token." };

            // Revoke old token
            storedToken.IsRevoked = true;
            await _unitOfWork.RefreshTokenRepository.UpdateAsync(storedToken);

            // Generate new tokens
            var newAccessToken = tokenService.CreateAccessToken(storedToken.User);
            var newRefreshToken = tokenService.CreateRefreshToken();

            var newTokenEntity = new RefreshToken
            {
                UserId = storedToken.UserId,
                TokenHash = TokenHasher.HashToken(newRefreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokenRepository.CreateAsync(newTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResultDTO
            {
                Success = true,
                Data = new { AccessToken = newAccessToken, RefreshToken = newRefreshToken }
            };
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var tokenHash = TokenHasher.HashToken(refreshToken);

            var storedToken = await _unitOfWork.RefreshTokenRepository.GetAsync(
                t => t.TokenHash == tokenHash && !t.IsRevoked
            );

            if (storedToken == null)
                return false;

            storedToken.IsRevoked = true;
            await _unitOfWork.RefreshTokenRepository.UpdateAsync(storedToken);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}