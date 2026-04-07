using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Reviews;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewRecipesController(IReviewRecipeService reviewService) : ControllerBase
{
    [HttpGet("/api/recipe/{recipeId}/reviews/get-me")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> GetMyReview(int recipeId, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var result = await reviewService.GetMyReviewAsync(userId, recipeId, cancellationToken);
        if (result == null)
            return NotFound(new { Message = "You haven't reviewed this recipe yet." });

        return Ok(result);
    }

    [HttpGet("/api/recipe/{recipeId}/reviews/all")]
    public async Task<IActionResult> GetRecipeReviews(int recipeId, CancellationToken cancellationToken)
    {
        // 👈 بنسأل الـ Context: هل اليوزر ده عامل لوجين؟ وهل الرول بتاعه عطار؟
        bool isHerbalist = User.Identity?.IsAuthenticated == true && User.IsInRole(AppRoles.Herbalist);

        // بنباصي النتيجة للـ Service
        var reviews = await reviewService.GetAllRecipeReviewsAsync(recipeId, isHerbalist, cancellationToken);

        return Ok(reviews);
    }

    [HttpPost("/api/recipe/{recipeId}/reviews/submit")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> SubmitReview(int recipeId, [FromBody] SubmitReviewRequest request,
        CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        try
        {
            var result = await reviewService.SubmitReviewAsync(userId, recipeId, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("/api/recipe/{recipeId}/reviews/delete-me")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> DeleteMyReview(int recipeId, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) 
            return Unauthorized();

        var success = await reviewService.DeleteMyReviewAsync(userId, recipeId, cancellationToken);
        if (!success) 
            return NotFound(new { Message = "Review not found." });

        return Ok(new { Message = "Review deleted successfully." });
    }
}