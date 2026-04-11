using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Accounts;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Services;


namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController] //kari
public class AccountsController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserAuthRequest model, 
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Register user using AuthService (which will hash password internally)
        var result = await authService.RegisterAsync(model, cancellationToken);

        if (!result.Success)
            return BadRequest(new { result.Message });

        return Ok(new { result.Message });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginAccountRequest model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authService.LoginAsync(model, cancellationToken);

        if (!result.Success)
            return Unauthorized(new { result.Message });

        return Ok(result.Data); // Contains accessToken + refreshToken
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordAccountRequest model)
    {
        // Reset password via UserService (it will hash the new password)
        var result = await authService.ResetPasswordAsync(model);

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordAccountRequest model,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authService.ForgotPasswordAsync(model, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest model, 
        CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(model.RefreshToken, cancellationToken);
        if (!result.Success) 
            return Unauthorized(new { result.Message });
        return Ok(result.Data);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest model, 
        CancellationToken cancellationToken)
    {
        var result = await authService.LogoutAsync(model.RefreshToken, cancellationToken);
        if (!result.Success) return BadRequest(new { result.Message });
        return Ok(new { result.Message });
    }
}