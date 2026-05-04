using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.UserFavorites;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Api.Extensions;
using PhytoIntellect.Application.Paginations;
namespace PhytoIntellect.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FavoritesController(IFavoriteService favoriteService) : ControllerBase
{
    [HttpGet("my-herbs")]
    public async Task<IActionResult> GetHerbs([FromQuery] RequestFilters filters)
    {
        var herbs = await favoriteService.GetMyFavoriteHerbsAsync(User.GetUserId(), filters);
        return Ok(herbs);
    }

    [HttpGet("my-recipes")]
    public async Task<IActionResult> GetRecipes([FromQuery] RequestFilters filters)
    {
        var recipes = await favoriteService.GetMyFavoriteRecipesAsync(User.GetUserId(), filters);
        return Ok(recipes);
    }

    [HttpGet("my-ai-recipes")]
    public async Task<IActionResult> GetAiRecipes([FromQuery] RequestFilters filters)
    {
        var aiRecipes = await favoriteService.GetMyFavoriteAiRecipesAsync(User.GetUserId(), filters);
        return Ok(aiRecipes);
    }

    [HttpGet("my-herbalists")]
    public async Task<IActionResult> GetHerbalists([FromQuery] RequestFilters filters)
    {
        var herbalists = await favoriteService.GetMyFavoriteHerbalistsAsync(User.GetUserId(), filters);
        return Ok(herbalists);
    }

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
}