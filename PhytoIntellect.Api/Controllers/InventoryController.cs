using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Inventory;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyInventory(CancellationToken cancellationToken)
    {
        int userId = 1; // لاحقاً من JWT

        var result = await inventoryService.GetMyInventoryAsync(userId, cancellationToken);

        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddHerbToInventoryRequest request, CancellationToken cancellationToken)
    {
        int userId = 1;

        var result = await inventoryService.AddHerbAsync(userId, request, cancellationToken);

        return Ok(result);
    }

    [HttpPut("/api/herbs/{herbId}/Inventory/update")]
    public async Task<IActionResult> Update(int herbId, [FromBody] UpdateInventoryRequest request, CancellationToken cancellationToken)
    {
        int userId = 1;

        var result = await inventoryService.UpdateInventoryAsync(userId, herbId, request, cancellationToken);

        return Ok(new { Message = "Inventory updated successfully." });
    }

    [HttpDelete("/api/herbs/{herbId}/Inventory/delete")]
    public async Task<IActionResult> Delete(int herbId, CancellationToken cancellationToken)
    {
        int userId = 1;

        var result = await inventoryService.RemoveHerbAsync(userId, herbId, cancellationToken);

        return Ok(new { Message = "Item removed from inventory successfully." });
    }
}