using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Accounts;
using PhytoIntellect.Application.Interfaces;
using ResendConfirmationEmailRequest = PhytoIntellect.Application.Contracts.Accounts.ResendConfirmationEmailRequest;


namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserAuthRequest model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authService.RegisterAsync(model, isAddedByAdmin: false, cancellationToken);

        if (!result.Success)
            return BadRequest(new { result.Message });

        return Ok(new { result.Message });
    }

    [AllowAnonymous]
    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { Message = "Email is required." });

        var result = await authService.ResendConfirmationEmailAsync(request.Email);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
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

        return Ok(result.Data); 
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    // this work with forget and reset password
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordAccountRequest model)
    {
        var result = await authService.ResetPasswordAsync(model);

        if (!result.Success)
            return BadRequest(result.Message);

        return Content("""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Password Reset Success</title>
</head>

<body style='font-family:Arial;text-align:center;padding-top:50px;background:#f9f9f9'>

    <h2 style='color:green'>✅ Password reset successfully</h2>

    <p>You can now login with your new password.</p>

</body>
</html>
""", "text/html");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordAccountRequest model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authService.ForgotPasswordAsync(model, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest model, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(model.RefreshToken, cancellationToken);
        if (!result.Success) 
            return Unauthorized(new { result.Message });
        return Ok(result.Data);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest model, CancellationToken cancellationToken)
    {
        var result = await authService.LogoutAsync(model.RefreshToken, cancellationToken);
        if (!result.Success) return BadRequest(new { result.Message });
        return Ok(new { result.Message });
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var result = await authService.GoogleLoginAsync(request);
            return Ok(result);
        }
        catch (InvalidJwtException)
        {
            return Unauthorized(new { Message = "Invalid Google Token" });
        }
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            return BadRequest("Invalid request parameters.");

        var result = await authService.ConfirmEmailAsync(email, token, cancellationToken);

        if (result.Success)
        {
            var successHtml = """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Email Confirmed</title>
            </head>
            <body style="text-align:center; padding-top:100px; font-family:Arial, sans-serif; background-color:#f9f9f9;">
                <h1 style="color:#28a745;">✅ Email Confirmed Successfully!</h1>
                <p style="font-size:18px; color:#555;">Your account has been verified. You can now close this window and return to the app to log in.</p>
            </body>
            </html>
            """;

            return Content(successHtml, "text/html");
        }
        else
        {
            var errorHtml = $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Confirmation Failed</title>
            </head>
            <body style="text-align:center; padding-top:100px; font-family:Arial, sans-serif; background-color:#f9f9f9;">
                <h1 style="color:#dc3545;">❌ Email Confirmation Failed</h1>
                <p style="font-size:18px; color:#555;">{result.Message}</p>
            </body>
            </html>
            """;

            return Content(errorHtml, "text/html");
        }
    }

    [AllowAnonymous]
    [HttpGet("reset-password")]
    public async Task<IActionResult> ResetPasswordPage([FromQuery] string email,[FromQuery] string token)
    {
        var result = await authService.ValidateResetTokenAsync(email, token);

        if (!result.Success)
        {
            return Content($"""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Invalid Reset Link</title>
</head>

<body style='font-family:Arial;text-align:center;padding-top:50px;background:#f9f9f9'>

    <h2 style='color:red'>❌ Invalid or expired link</h2>

    <p>{result.Message}</p>

</body>
</html>
""", "text/html");
        }

        var html = $"""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Reset Password</title>
</head>

<body style='font-family:Arial;text-align:center;padding-top:50px;background:#f9f9f9'>

    <h2>Reset Password 🔐</h2>

    <form method='post' action='/api/accounts/reset-password'>

        <input type='hidden' name='Email' value='{email}' />
        <input type='hidden' name='Token' value='{token}' />

        <div style='margin-bottom:15px'>
            <input
                type='password'
                name='NewPassword'
                placeholder='Enter new password'
                required
                style='padding:10px;width:250px;border:1px solid #ccc;border-radius:5px'/>
        </div>

        <button type='submit'
                style='padding:10px 20px;background:#dc3545;color:white;border:none;border-radius:5px;cursor:pointer'>
            Reset Password
        </button>

    </form>

</body>
</html>
""";

        return Content(html, "text/html");
    }
}