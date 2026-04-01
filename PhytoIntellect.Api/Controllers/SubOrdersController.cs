using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Core.Entities;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Herbalist)]
public class SubOrdersController(ISubOrderService subOrderService) : ControllerBase
{
    private readonly ISubOrderService _subOrderService = subOrderService;

    [HttpGet("my-tasks")]
    public async Task<IActionResult> GetMyTasks(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tasks = await _subOrderService.GetHerbalistSubOrdersAsync(userId!, cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{subOrderId}")]
    public async Task<IActionResult> GetSubOrderDetails(int subOrderId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var details = await _subOrderService.GetSubOrderDetailsAsync(subOrderId, userId!, cancellationToken);
        return Ok(details);
    }

    [HttpPut("{subOrderId}/status")]
    public async Task<IActionResult> UpdateStatus(int subOrderId, [FromBody] UpdateSubOrderStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new { Message = "User is not logged in." });

            await subOrderService.UpdateSubOrderStatusAsync(subOrderId, userId, request, cancellationToken);

            return Ok(new { Message = "SubOrder status updated successfully." });
        }
        catch (ArgumentException ex)
        {
            // 👈 بيمسك الداتا الغلط (زي حالة غلط أو يوزر ID غلط) ويرجع 400 Bad Request
            return BadRequest(new { ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            // 👈 بيمسك محاولة الدخول الممنوعة ويرجع 403 Forbidden
            return StatusCode(403, new { ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            // 👈 بيمسك الأوردر اللي مش موجود أو مش بتاعه ويرجع 404 Not Found
            return NotFound(new { ex.Message });
        }
        catch (Exception ex)
        {
            // 👈 أي إيرور تاني سيرفر ويرجع 500
            return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
        }
    }
}