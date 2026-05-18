using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Reviews;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/ai-chat-recipe/{id}/reviews")]
public class AiChatRecipeReviewsController(IReviewRecipeService reviewService) : ControllerBase
{
    [HttpGet("get-me")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> GetMyReview(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var result = await reviewService.GetMyAiChatReviewAsync(userId, id, cancellationToken);
        if (result == null)
            return NotFound(new { Message = "You haven't reviewed this AI Chat recipe yet." });

        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAiChatRecipeReviews(int id, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var reviews = await reviewService.GetAllAiChatRecipeReviewsAsync(id, filters, cancellationToken);
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
            var result = await reviewService.SubmitAiChatReviewAsync(userId, id, request, cancellationToken);
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

        var success = await reviewService.DeleteMyAiChatReviewAsync(userId, id, cancellationToken);
        if (!success)
            return NotFound(new { Message = "Review not found." });

        return Ok(new { Message = "Review deleted successfully." });
    }


    // [Admin] Get All Global Chat Reviews
    [HttpGet("~/api/admin/ai-chat-recipe-reviews")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> AdminGetAllChatReviews([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var reviews = await reviewService.GetAllSystemAiChatReviewsAsync(filters, cancellationToken);
        return Ok(reviews);
    }

    // [Admin] Delete Any Chat Review
    [HttpDelete("~/api/admin/ai-chat-recipe-reviews/{reviewId}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> AdminDeleteChatReview(int reviewId, CancellationToken cancellationToken)
    {
        var success = await reviewService.DeleteAnyAiChatReviewAsync(reviewId, cancellationToken);
        if (!success)
            return NotFound(new { Message = "Review not found." });

        return Ok(new { Message = "Review deleted successfully by Admin." });
    }

} 