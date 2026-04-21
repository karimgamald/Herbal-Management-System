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
[Route("api/ai-recipe/{id}/reviews")]
public class ReviewRecipesController(IReviewRecipeService reviewService) : ControllerBase
{
    [HttpGet("get-me")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> GetMyReview(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var result = await reviewService.GetMyReviewAsync(userId, id, cancellationToken);
        if (result == null)
            return NotFound(new { Message = "You haven't reviewed this AI recipe yet." });

        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAiRecipeReviews(int id, CancellationToken cancellationToken)
    {
        var reviews = await reviewService.GetAllRecipeReviewsAsync(id, cancellationToken);
        return Ok(reviews);
    }

    [HttpPost("submit")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> SubmitReview(int id, [FromBody] SubmitReviewRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        try
        {
            var result = await reviewService.SubmitReviewAsync(userId, id, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("delete-me")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> DeleteMyReview(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var success = await reviewService.DeleteMyReviewAsync(userId, id, cancellationToken);
        if (!success)
            return NotFound(new { Message = "Review not found." });

        return Ok(new { Message = "Review deleted successfully." });
    }
}