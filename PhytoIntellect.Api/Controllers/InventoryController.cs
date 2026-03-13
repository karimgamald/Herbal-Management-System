using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Inventory;

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

    [HttpPost]
    public async Task<IActionResult> Add(AddHerbToInventoryRequest request,CancellationToken cancellationToken)
    {
        int userId = 1;

        var result = await inventoryService.AddHerbAsync(userId, request, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{herbId}")]
    public async Task<IActionResult> Update(int herbId,UpdateInventoryRequest request,CancellationToken cancellationToken)
    {
        int userId = 1;

        var result = await inventoryService.UpdateInventoryAsync(userId, herbId, request, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{herbId}")]
    public async Task<IActionResult> Delete(int herbId, CancellationToken cancellationToken)
    {
        int userId = 1;

        var result = await inventoryService.RemoveHerbAsync(userId, herbId, cancellationToken);

        return Ok(result);
    }
}