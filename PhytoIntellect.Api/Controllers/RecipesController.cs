using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Recipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipesController(IRecipeService recipeService) : ControllerBase
{
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

    [HttpGet("all")]
    public async Task<IActionResult> GetAllRecipes(CancellationToken cancellationToken)
    {
        var recipes = await recipeService.GetAllActiveRecipesAsync(cancellationToken);
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

    [Authorize(Roles = "Herbalist")]
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
            return StatusCode(403, new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize(Roles = "Herbalist")]
    [HttpDelete("{id}/delete")]
    public async Task<IActionResult> DeleteRecipe(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) 
            return Unauthorized();

        var success = await recipeService.DeleteRecipeAsync(userId, id, cancellationToken);

        if (!success) 
            return BadRequest(new { Message = "Failed to delete recipe. It may not exist or you don't have permission." });

        return Ok(new { Message = "Recipe deleted successfully." });
    }
}