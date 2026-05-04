using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Inventory;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    private int GetHerbalistId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) throw new UnauthorizedAccessException("User is not authenticated.");
        return int.Parse(userIdClaim.Value);
    }

    [HttpGet("me")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> GetMyInventory([FromQuery] RequestFilters filters,CancellationToken cancellationToken)
    {
        try
        {
            var herbalistId = GetHerbalistId();
            var result = await inventoryService.GetMyInventoryAsync(herbalistId, filters,cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("{id}/get-id")] 
    public async Task<IActionResult> GetAllByHerbalistId([FromRoute] int id,[FromQuery] RequestFilters filters, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Add(AddHerbToInventoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var herbalistId = GetHerbalistId();
            var result = await inventoryService.AddHerbAsync(herbalistId, request, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}/update")] 
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> Update(int id, UpdateInventoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var herbalistId = GetHerbalistId();
            await inventoryService.UpdateInventoryAsync(herbalistId, id, request, cancellationToken);
            return Ok(new { message = "Inventory updated successfully." });
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpDelete("{id}/delete")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var herbalistId = GetHerbalistId();
            await inventoryService.RemoveHerbAsync(herbalistId, id, cancellationToken);
            return Ok(new { message = "Herb removed from inventory successfully." });
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}