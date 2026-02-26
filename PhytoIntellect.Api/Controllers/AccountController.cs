using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Api.DTOs.UserDTOs;
using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Application.Interfaces;

namespace PhytoIntellect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public AccountController(
            IUserService userService,
            IAuthService authService,
            ITokenService tokenService)
        {
            _userService = userService;
            _authService = authService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Register user using AuthService (which will hash password internally)
            var result = await _authService.RegisterAsync(model);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);

            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result.Data); // Contains accessToken + refreshToken
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            // Reset password via UserService (it will hash the new password)
            var result = await _userService.ResetPasswordAsync(model);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDTO model)
        {
            var result = await _authService.RefreshAsync(model.RefreshToken);

            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result.Data); // Contains new accessToken + refreshToken
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDTO model)
        {
            var result = await _authService.LogoutAsync(model.RefreshToken);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}