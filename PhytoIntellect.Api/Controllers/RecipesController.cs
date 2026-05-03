using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Recipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Core.Entities;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipesController(IRecipeService recipeService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAllRecipes([FromQuery] RequestFilters filters,CancellationToken cancellationToken)
    {
        var recipes = await recipeService.GetAllActiveRecipesAsync(filters,cancellationToken);
        return Ok(recipes);
    }

    [HttpGet("{id}/get-id")]
    public async Task<IActionResult> GetRecipeById(int id, CancellationToken cancellationToken)
    {
        var recipe = await recipeService.GetRecipeByIdAsync(id, cancellationToken);
        if (recipe == null)
            return NotFound(new { Message = "Recipe not found." });

        return Ok(recipe);
    }

    [HttpGet("herbalist/{id}")]
    public async Task<IActionResult> GetRecipesByHerbalist([FromRoute] int id,[FromQuery] RequestFilters filters, [FromQuery] bool? isActive, CancellationToken cancellationToken)
    {
        var recipes = await recipeService.GetRecipesByHerbalistIdAsync(id,filters, isActive, cancellationToken);

        return Ok(recipes);
    }

    [Authorize(Roles =AppRoles.Herbalist)]
    [HttpPost("add")]
    public async Task<IActionResult> AddRecipe([FromBody] CreateRecipeRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        try
        {
            var response = await recipeService.AddRecipeAsync(userId, request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpPut("{id}/update")]
    public async Task<IActionResult> UpdateRecipe(int id, [FromBody] UpdateRecipeRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) 
            return Unauthorized();

        try
        {
            var updatedRecipe = await recipeService.UpdateRecipeAsync(userId, id, request, cancellationToken);
            return Ok(updatedRecipe);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    [Authorize(Roles = "Herbalist")]
    [HttpPatch("{id}/toggle-availability")]
    public async Task<IActionResult> ToggleRecipeAvailability(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var newState = await recipeService.ToggleRecipeAvailabilityAsync(userId, id, cancellationToken);

        if (newState == null)
            return BadRequest(new { Message = "Failed to update recipe. It may not exist or you don't have permission to modify it." });

        string responseMessage = newState == true
            ? "Recipe activated successfully. It is now visible to patients."
            : "Recipe deactivated successfully. It is no longer visible to patients.";

        return Ok(new
        {
            Message = responseMessage,
            IsActive = newState
        });
    }
} 