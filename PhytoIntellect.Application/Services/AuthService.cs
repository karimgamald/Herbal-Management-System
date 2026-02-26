using BCrypt.Net;
using PhytoIntellect.Api.DTOs.UserDTOs;
using PhytoIntellect.Application.DTOs;
using PhytoIntellect.Application.DTOs.PhytoIntellect.Application.DTOs;
using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthService(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        // --------------------- REGISTER ---------------------
        public async Task<AuthResultDTO> RegisterAsync(RegisterUserDTO model)
        {
            // Check if username exists
            var existingUser = await _userService.ValidateByUserNameAsync(model.UserName);
            if (existingUser != null)
                return new AuthResultDTO { Success = false, Message = "Username already exists" };

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Create user entity
            var user = new User
            {
                UserName = model.UserName,
                PasswordHash = hashedPassword,
                Role = model.Role
            };

            await _userService.AddUserAsync(user);

            return new AuthResultDTO { Success = true, Message = "User registered successfully" };
        }

        // --------------------- LOGIN ---------------------
        public async Task<AuthResultDTO> LoginAsync(UserDTO model)
        {
            var user = await _userService.ValidateUserAsync(model.UserName, model.Password);
            if (user == null)
                return new AuthResultDTO { Success = false, Message = "Invalid username or password" };

            var accessToken = _tokenService.CreateAccessToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();

            // Save refresh token via UserService
            await _userService.AddRefreshTokenAsync(user.Id, refreshToken);

            return new AuthResultDTO
            {
                Success = true,
                Data = new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }

        // --------------------- REFRESH ---------------------
        public async Task<AuthResultDTO> RefreshAsync(string refreshToken)
        {
            var result = await _userService.RefreshTokenAsync(refreshToken, _tokenService);
            if (!result.Success)
                return new AuthResultDTO { Success = false, Message = "Invalid or expired refresh token" };

            return result;
        }

        // --------------------- LOGOUT ---------------------
        public async Task<AuthResultDTO> LogoutAsync(string refreshToken)
        {
            var revoked = await _userService.RevokeRefreshTokenAsync(refreshToken);

            if (!revoked)
                return new AuthResultDTO { Success = false, Message = "Token not found or already revoked" };

            return new AuthResultDTO { Success = true, Message = "Logged out successfully" };
        }
    }
}