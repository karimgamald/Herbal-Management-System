using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Reviews;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
// 👈 المسار بقى صريح: خاص بوصفات الذكاء الاصطناعي فقط
[Route("api/ai-recipe/{aiRecipeId}/reviews")]
public class ReviewRecipesController(IReviewRecipeService reviewService) : ControllerBase
{
    [HttpGet("get-me")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> GetMyReview(int aiRecipeId, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var result = await reviewService.GetMyReviewAsync(userId, aiRecipeId, cancellationToken);
        if (result == null)
            return NotFound(new { Message = "You haven't reviewed this AI recipe yet." });

        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAiRecipeReviews(int aiRecipeId, CancellationToken cancellationToken)
    {
        var reviews = await reviewService.GetAllRecipeReviewsAsync(aiRecipeId, cancellationToken);
        return Ok(reviews);
    }

    [HttpPost("submit")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> SubmitReview(int aiRecipeId, [FromBody] SubmitReviewRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        try
        {
            var result = await reviewService.SubmitReviewAsync(userId, aiRecipeId, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("delete-me")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> DeleteMyReview(int aiRecipeId, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var success = await reviewService.DeleteMyReviewAsync(userId, aiRecipeId, cancellationToken);
        if (!success)
            return NotFound(new { Message = "Review not found." });

        return Ok(new { Message = "Review deleted successfully." });
    }
}