using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Api.Extensions;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
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
    public async Task<IActionResult> GetMyTasks([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tasks = await _subOrderService.GetHerbalistSubOrdersAsync(userId!, filters,cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubOrderDetails(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var details = await _subOrderService.GetSubOrderDetailsAsync(id, userId!, cancellationToken);
        return Ok(details);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateSubOrderStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) 
                return Unauthorized(new { Message = "User is not logged in." });

            await subOrderService.UpdateSubOrderStatusAsync(id, userId, request, cancellationToken);

            return Ok(new { Message = "SubOrder status updated successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { ex.Message }); 
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
        }
    }

    [HttpGet("my-financials")]
    public async Task<IActionResult> GetFinancialDashboard(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId().ToString();

        var dashboardData = await subOrderService.GetHerbalistFinancialsAsync(userId, cancellationToken);

        return Ok(dashboardData);
    }
}