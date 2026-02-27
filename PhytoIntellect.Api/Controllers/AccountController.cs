using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.DTOs.AuthDTOs;
using PhytoIntellect.Application.Interfaces;


namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserAuthDto model, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(model, cancellationToken);
        if (!result.Success) return BadRequest(new { result.Message });
        return Ok(new { result.Message });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(model, cancellationToken);
        if (!result.Success) return Unauthorized(new { result.Message });
        return Ok(result.Data);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenRequestDto model, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(model.RefreshToken, cancellationToken);
        if (!result.Success) return Unauthorized(new { result.Message });
        return Ok(result.Data);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] TokenRequestDto model, CancellationToken cancellationToken)
    {
        var result = await authService.LogoutAsync(model.RefreshToken, cancellationToken);
        if (!result.Success) return BadRequest(new { result.Message });
        return Ok(new { result.Message });
    }
}