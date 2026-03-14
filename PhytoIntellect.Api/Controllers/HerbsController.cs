
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Herbs;
using System.Security.Claims; 
namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HerbsController(IHerbService herbService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var herbs = await herbService.GetApprovedHerbsAsync(cancellationToken);
        return Ok(herbs);
    }

    [HttpGet("{id}/get-id")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var herb = await herbService.GetHerbByIdAsync(id, cancellationToken);

        if (herb == null)
            return NotFound("Herb not found.");

        return Ok(herb);
    }

    [HttpPost("add")]
    // ضيف هنا الـ Authorize لو عاوز العطارين بس اللي يضيفوا
    public async Task<IActionResult> Create([FromForm] HerbRequest request, CancellationToken cancellationToken)
    {
        // 👈 وحدنا الطريقة عشان تجيب الـ UserId من التوكن
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized(new { Message = "Invalid user token." });

        try
        {
            var result = await herbService.CreateHerbAsync(userId, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}/update")]
    public async Task<IActionResult> Update(int id, [FromForm] HerbRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized(new { Message = "Invalid user token." });

        try
        {
            var result = await herbService.UpdateHerbAsync(userId, id, request, cancellationToken);

            if (result == null)
                return NotFound(new { Message = "Herb not found or you don't have permission to update it." });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("{id}/delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await herbService.DeleteHerbAsync(id, cancellationToken);
        if (!result) return NotFound(new { Message = "Herb not found." });

        return Ok(new { Message = "Herb deleted successfully." });
    }
}