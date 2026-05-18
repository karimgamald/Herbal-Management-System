using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Api.Extensions; 
using PhytoIntellect.Application.Contracts.Inventory;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;

namespace PhytoIntellect.API.Controllers;

[ApiController]
[Route("api/inventory-herbs")] 
public class InventoryHerbsController(IInventoryHerbsService inventoryService) : ControllerBase
{
    [HttpGet("my-inventory")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> GetMyInventory([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        try
        {
            var herbalistId = User.GetUserId(); 
            var result = await inventoryService.GetMyInventoryAsync(herbalistId, filters, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("{id:int}/herbalists")] 
    [AllowAnonymous]
    public async Task<IActionResult> GetAllByHerbalistId([FromRoute] int id, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        try
        {
            var result = await inventoryService.GetAllByHerbalistIdAsync(id, filters, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost("add")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> Add([FromBody] AddHerbToInventoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var herbalistId = User.GetUserId();
            var result = await inventoryService.AddHerbAsync(herbalistId, request, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}/update")] 
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateInventoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var herbalistId = User.GetUserId();
            await inventoryService.UpdateInventoryAsync(herbalistId, id, request, cancellationToken);
            return Ok(new { message = "Inventory updated successfully." });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpDelete("{id:int}/delete")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            var herbalistId = User.GetUserId();
            await inventoryService.RemoveHerbAsync(herbalistId, id, cancellationToken);
            return Ok(new { message = "Herb removed from inventory successfully." });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    // ================= Admin Endpoints =================

    [HttpGet("~/api/admin/inventory-herbs")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAllInventoryByAdmin([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var result = await inventoryService.GetAllInventoryByAdminAsync(filters, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("~/api/admin/inventory-herbs/{herbalistId:int}/{herbId:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> RemoveFromInventoryByAdmin([FromRoute] int herbalistId, [FromRoute] int herbId, CancellationToken cancellationToken)
    {
        var success = await inventoryService.RemoveHerbByAdminAsync(herbalistId, herbId, cancellationToken);
        if (!success)
            return NotFound(new { Message = "Inventory item not found." });

        return Ok(new { Message = "Herb removed from herbalist's inventory by Admin." });
    }
}