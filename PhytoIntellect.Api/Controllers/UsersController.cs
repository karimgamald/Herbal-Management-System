using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Accounts;
using PhytoIntellect.Application.Contracts.Users;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Application.Services;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Infrastructure.Identities;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.Admin)]  
public class UsersController(IUserService userService, IAuthService authService) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllUsers([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var users = await userService.GetAllUsersAsync(filters, cancellationToken);
        return Ok(users);
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByIdAsync(id, cancellationToken);
        if (user == null) 
            return NotFound(new { Message = "User not found." });
        return Ok(user);
    }


    [HttpPost("add-admin")]
    public async Task<IActionResult> AddUser([FromBody] RegisterUserAuthRequest model, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(model, isAddedByAdmin: true, cancellationToken);

        if (!result.Success)
            return BadRequest(new { Message = result.Message });

        return Ok(new { Message = result.Message });
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request,CancellationToken cancellationToken)
    {
        var result = await userService.UpdateUserAsync(id, request, cancellationToken);
        if (result.Contains("Invalid") || result.Contains("not found")) 
            return BadRequest(new { Message = result });
        return Ok(new { Message = result });
    }

    [Authorize]
    [HttpPatch("update-my-address")]
    public async Task<IActionResult> UpdateAddress([FromBody] UpdateUserAddressRequest model,CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        var result = await userService.UpdateAddressAsync(userId, model, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
    {
        var result = await userService.DeleteUserAsync(id, cancellationToken);
        if (result == "User not found.") 
            return NotFound(new { Message = result });
        return Ok(new { Message = result });
    }

}