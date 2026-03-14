using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Inventory;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Herbalist)]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyInventory(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var herbalistId = int.Parse(userIdClaim.Value);

        var result = await inventoryService.GetMyInventoryAsync(herbalistId, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add( AddHerbToInventoryRequest request,CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var herbalistId = int.Parse(userIdClaim.Value);

        var result = await inventoryService.AddHerbAsync(herbalistId, request, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{herbId}")]
    public async Task<IActionResult> Update(int herbId,UpdateInventoryRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var herbalistId = int.Parse(userIdClaim.Value);

        var result = await inventoryService.UpdateInventoryAsync(herbalistId, herbId, request, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{herbId}")]
    public async Task<IActionResult> Delete(int herbId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var herbalistId = int.Parse(userIdClaim.Value);

        var result = await inventoryService.RemoveHerbAsync(herbalistId, herbId, cancellationToken);

        return Ok(result);
    }
}