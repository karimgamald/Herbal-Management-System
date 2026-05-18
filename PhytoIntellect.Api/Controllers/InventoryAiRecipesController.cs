using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Api.Extensions;
using PhytoIntellect.Application.Contracts.HerbalistAiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;

namespace PhytoIntellect.Api.Controllers;

[Route("api/inventory-ai-recipes")]
[ApiController]
public class InventoryAiRecipesController(IHerbalistAiRecipeService inventoryService) : ControllerBase
{
    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpGet("my-inventory")]
    public async Task<IActionResult> GetMyInventory([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await inventoryService.GetMyInventoryAsync(userId, filters, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpPost("add")]
    public async Task<IActionResult> AddToInventory([FromBody] AddAiRecipeToInventoryRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await inventoryService.AddAiRecipeAsync(userId, request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpPatch("{id:int}/price")]
    public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdatePriceRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await inventoryService.UpdatePriceAsync(userId, id, request.Price, cancellationToken);
        return Ok(new { Message = "Price updated successfully." });
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ToggleStatus(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var newState = await inventoryService.ToggleStatusAsync(userId, id, cancellationToken);

        return Ok(new
        {
            Message = newState ? "Recipe activated successfully." : "Recipe deactivated successfully.",
            IsActive = newState
        });
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpDelete("{id:int}/delete")]
    public async Task<IActionResult> RemoveFromInventory(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await inventoryService.RemoveAiRecipeAsync(userId, id, cancellationToken);
        return Ok(new { Message = "Recipe removed from inventory." });
    }

    [HttpGet("{id:int}/herbalists")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHerbalistsForAiRecipe([FromRoute] int id,
        [FromQuery] bool isActive = true, CancellationToken cancellationToken = default)
    {
        var result = await inventoryService.GetHerbalistsByAiRecipeAsync(id, isActive, cancellationToken);

        if (!result.Any())
            return NotFound(new { Message = "No herbalists currently offer this recipe." });

        return Ok(result);
    }

    // ================= Admin Endpoints =================

    [HttpGet("~/api/admin/inventory-ai-recipes")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAllInventoryByAdmin([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var result = await inventoryService.GetAllAiRecipeInventoryByAdminAsync(filters, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("~/api/admin/inventory-ai-recipes/{herbalistId:int}/{recipeId:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> RemoveFromInventoryByAdmin([FromRoute] int herbalistId, [FromRoute] int recipeId, CancellationToken cancellationToken)
    {
        var success = await inventoryService.RemoveAiRecipeByAdminAsync(herbalistId, recipeId, cancellationToken);
        if (!success)
            return NotFound(new { Message = "Inventory item not found." });

        return Ok(new { Message = "Recipe removed from herbalist's inventory by Admin." });
    }
}