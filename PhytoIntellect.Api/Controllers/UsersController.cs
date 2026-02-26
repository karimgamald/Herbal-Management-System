using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Application.Interfaces;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    //[HttpPost("register")]
    //public async Task<IActionResult> Register([FromBody] RegisterUserDTO request)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return BadRequest(ModelState);
    //    }

    //    var result = await _userService.ValidateUserAsync(request.UserName, request.Password);

    //    if (result == null)
    //    {
    //        return BadRequest(new { Message = result });
    //    }

    //    return Ok(new { Message = result });
    //}
}