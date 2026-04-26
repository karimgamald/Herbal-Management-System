using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.UserFavorites;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Api.Extensions;
namespace PhytoIntellect.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FavoritesController(IFavoriteService favoriteService) : ControllerBase
{
    [HttpPost("toggle")]
    public async Task<IActionResult> Toggle([FromBody] ToggleFavoriteRequest request)
    {
        var result = await favoriteService.ToggleFavoriteAsync(User.GetUserId(), request);

        if (result == "Target not found.")
        {
            return NotFound(new { Message = $"The specified {request.Type} does not exist in the system." });
        }

        return Ok(new { Message = result });
    }

    [HttpGet("my-herbs")]
    public async Task<IActionResult> GetHerbs()
    {
        var herbs = await favoriteService.GetMyFavoriteHerbsAsync(User.GetUserId());
        return Ok(herbs);
    }

    [HttpGet("my-recipes")]
    public async Task<IActionResult> GetRecipes()
    {
        var recipes = await favoriteService.GetMyFavoriteRecipesAsync(User.GetUserId());
        return Ok(recipes);
    }

    [HttpGet("my-ai-recipes")]
    public async Task<IActionResult> GetAiRecipes()
    {
        var aiRecipes = await favoriteService.GetMyFavoriteAiRecipesAsync(User.GetUserId());
        return Ok(aiRecipes);
    }

    [HttpGet("my-herbalists")]
    public async Task<IActionResult> GetHerbalists()
    {
        var herbalists = await favoriteService.GetMyFavoriteHerbalistsAsync(User.GetUserId());
        return Ok(herbalists);
    }
}