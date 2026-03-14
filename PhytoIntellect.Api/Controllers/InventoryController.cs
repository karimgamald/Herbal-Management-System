using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Inventory;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Herbalist")] // 👈 المخزن ده للعطارين بس!
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyInventory(CancellationToken cancellationToken)
    {
        // بنقرأ الـ ID من التوكن
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var result = await inventoryService.GetMyInventoryAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddHerbToInventoryRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        try
        {
            var result = await inventoryService.AddHerbAsync(userId, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("/api/herbs/{herbId}/Inventory/update")]
    public async Task<IActionResult> Update(int herbId, [FromBody] UpdateInventoryRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var success = await inventoryService.UpdateInventoryAsync(userId, herbId, request, cancellationToken);
        if (!success) return NotFound(new { Message = "Item not found in your inventory." });

        return Ok(new { Message = "Inventory updated successfully." });
    }

    [HttpDelete("/api/herbs/{herbId}/Inventory/delete")]
    public async Task<IActionResult> Delete(int herbId, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var success = await inventoryService.RemoveHerbAsync(userId, herbId, cancellationToken);
        if (!success) return NotFound(new { Message = "Item not found in your inventory." });

        return Ok(new { Message = "Item removed from inventory successfully." });
    }
}