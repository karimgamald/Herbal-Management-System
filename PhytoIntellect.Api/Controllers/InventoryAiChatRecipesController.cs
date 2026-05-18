using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Api.Extensions;
using PhytoIntellect.Application.Contracts.HerbalistAiChatRecipes;
using PhytoIntellect.Application.Contracts.HerbalistAiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;

namespace PhytoIntellect.Api.Controllers;

[Route("api/inventory-ai-chat-recipes")]
[ApiController]
public class InventoryAiChatRecipesController(IHerbalistAiChatRecipeService inventoryService) : ControllerBase
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
    public async Task<IActionResult> AddToInventory([FromBody] AddAiChatRecipeToInventoryRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await inventoryService.AddAiChatRecipeAsync(userId, request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpPatch("{id}/price")]
    public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdatePriceRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await inventoryService.UpdatePriceAsync(userId, id, request.Price, cancellationToken);
        return Ok(new { Message = "Price updated successfully." });
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ToggleStatus(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var newState = await inventoryService.ToggleStatusAsync(userId, id, cancellationToken);

        return Ok(new
        {
            Message = newState ? "Chat Recipe activated successfully." : "Chat Recipe deactivated successfully.",
            IsActive = newState
        });
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpDelete("{id}/delete")]
    public async Task<IActionResult> RemoveFromInventory(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await inventoryService.RemoveAiChatRecipeAsync(userId, id, cancellationToken);
        return Ok(new { Message = "Chat Recipe removed from inventory." });
    }

    [HttpGet("{id}/herbalists")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHerbalistsForAiChatRecipe([FromRoute] int id, [FromQuery] bool isActive = true, CancellationToken cancellationToken = default)
    {
        var result = await inventoryService.GetHerbalistsByAiChatRecipeAsync(id, isActive, cancellationToken);

        if (!result.Any())
            return NotFound(new { Message = "No herbalists currently offer this chat recipe." });

        return Ok(result);
    }

    // Admin Endpoints
    [HttpGet("~/api/admin/inventory-ai-chat-recipes")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> AdminGetAllInventory([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var result = await inventoryService.AdminGetAllInventoryAsync(filters, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("~/api/admin/inventory-ai-chat-recipes/{herbalistId}/{recipeId}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> AdminRemoveFromInventory(int herbalistId, int recipeId, CancellationToken cancellationToken)
    {
        var success = await inventoryService.AdminRemoveAiChatRecipeAsync(herbalistId, recipeId, cancellationToken);
        if (!success)
            return NotFound(new { Message = "Inventory item not found." });

        return Ok(new { Message = "Chat Recipe removed from herbalist's inventory by Admin." });
    }
}