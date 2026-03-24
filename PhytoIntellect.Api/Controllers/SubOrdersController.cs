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
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _subOrderService.UpdateSubOrderStatusAsync(subOrderId, userId!, request, cancellationToken);
        return Ok(new { Message = "Status updated successfully." });
    }
}