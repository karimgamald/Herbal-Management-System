using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Services;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
// تقدر قدام تفك الكومنت ده عشان تخلي الإدارة بس اللي تدخل هنا
// [Authorize(Roles = "Admin")] 
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await userService.GetAllUsersAsync(cancellationToken);
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

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto request,CancellationToken cancellationToken)
    {
        var result = await userService.UpdateUserAsync(id, request, cancellationToken);
        if (result.Contains("Invalid") || result.Contains("not found")) 
            return BadRequest(new { Message = result });
        return Ok(new { Message = result });
    }

    [Authorize]
    [HttpPatch("update-my-address")]
    public async Task<IActionResult> UpdateAddress([FromBody] UpdateUserAddressDto model,CancellationToken cancellationToken)
    {
        // get the userid from claims
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