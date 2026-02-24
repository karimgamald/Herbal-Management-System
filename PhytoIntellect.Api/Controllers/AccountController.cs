using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Api.DTOs.UserDTOs;
using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.Presistence;
using PhytoIntellect.Infrastructure.Repository;

namespace PhytoIntellect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;

        public AccountController(IUserService userService, ITokenService tokenService, ApplicationDbContext context)
        {
            _userService = userService;
            _tokenService = tokenService;
            _context = context;
        }

        //Allow anonymous access
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1️⃣ Check if username already exists
            if (await _context.Users.AnyAsync(u => u.UserName == model.UserName))
                return BadRequest("Username already exists");

            // 2️⃣ check the password and confirm password
            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest("Password and ConfirmPassword do not match");
            }

            // 3 Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // 4 Create user entity
            var user = new User
            {
                UserName = model.UserName,
                PasswordHash = hashedPassword,
                Role = model.Role
            };

            // 5 Save to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        //Allow anonymous access
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Validate user credentials
            var user = await _userService.ValidateUserAsync(
                model.UserName, model.Password);

            if (user == null)
                return Unauthorized("Invalid username or password");

            var accessToken = _tokenService.CreateAccessToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = TokenHasher.HashToken(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            //Return token
            return Ok(new
            {
                accessToken,
                refreshToken
            });

        }
        [HttpPost("Reset Password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            var user = await _userService.ValidateByUserNameAsync(model.UserName);

            if (user == null)
                return BadRequest("User not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            _context.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Password reset successfully.");
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshRequestDTO model)
        {
            var tokenHash = TokenHasher.HashToken(model.RefreshToken);

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == tokenHash &&
                    !x.IsRevoked &&
                    x.ExpiresAt > DateTime.UtcNow);

            if (storedToken == null)
                return Unauthorized();

            // revoke old token
            storedToken.IsRevoked = true;

            // Generate new tokens
            var newAccessToken = _tokenService.CreateAccessToken(storedToken.User);
            var newRefreshToken = _tokenService.CreateRefreshToken();

            // rotate refresh token
            storedToken.IsRevoked = true;

            _context.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = TokenHasher.HashToken(newRefreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshRequestDTO model)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == model.RefreshToken);

            if (token == null)
                return BadRequest();

            token.IsRevoked = true;
            await _context.SaveChangesAsync();

            return Ok("Logged out");
        }
    }
}
