using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Api.Extensions;
using PhytoIntellect.Application.Contracts.HerbalistAiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.Herbalist)]
public class InventoryAiRecipesController(IHerbalistAiRecipeService inventoryService) : ControllerBase
{
    [HttpGet("my-inventory")]
    public async Task<IActionResult> GetMyInventory([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await inventoryService.GetMyInventoryAsync(userId, filters, cancellationToken);
        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToInventory([FromBody] AddAiRecipeToInventoryRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await inventoryService.AddAiRecipeAsync(userId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id}/price")]
    public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdatePriceRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await inventoryService.UpdatePriceAsync(userId, id, request.Price, cancellationToken);
        return Ok(new { Message = "Price updated successfully." });
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ToggleStatus(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var newState = await inventoryService.ToggleStatusAsync(userId, id, cancellationToken);

        return Ok(new
        {
            Message = newState ?   "Recipe activated successfully." : "Recipe deactivated successfully.",
            IsActive = newState
        });
    }

    [HttpDelete("{id}/delete")]
    public async Task<IActionResult> RemoveFromInventory(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await inventoryService.RemoveAiRecipeAsync(userId, id, cancellationToken);
        return Ok(new { Message = "Recipe removed from inventory." });
    }

    [HttpGet("{id}/herbalists")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHerbalistsForAiRecipe([FromRoute] int id, 
        [FromQuery] bool isActive = true, CancellationToken cancellationToken = default)
    {
        var result = await inventoryService.GetHerbalistsByAiRecipeAsync(id, isActive, cancellationToken);

        if (!result.Any())
            return NotFound(new { Message = "No herbalists currently offer this recipe." });

        return Ok(result);
    }
}
