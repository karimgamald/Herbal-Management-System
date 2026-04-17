using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Feedbacks;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class FeedbacksController(IFeedbackService feedbackService) : ControllerBase
{
    // (Regular Recipes)
    [HttpGet("recipe/{recipeId}/all")]
    public async Task<IActionResult> GetRecipeFeedbacks(int recipeId, CancellationToken cancellationToken)
    {
        var feedbacks = await feedbackService.GetRecipeFeedbacksAsync(recipeId, cancellationToken);
        return Ok(feedbacks);
    }

    [HttpGet("recipe/{recipeId}/get-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> GetMyRecipeFeedback(int recipeId, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var result = await feedbackService.GetMyRecipeFeedbackAsync(userId, recipeId, cancellationToken);
        if (result == null) return NotFound(new { Message = "You haven't rated this recipe yet." });

        return Ok(result);
    }

    [HttpPost("recipe/{recipeId}/submit")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> SubmitRecipeFeedback(int recipeId, [FromBody] SubmitFeedbackRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        try
        {
            var result = await feedbackService.SubmitRecipeFeedbackAsync(userId, recipeId, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("recipe/{recipeId}/delete-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> DeleteMyRecipeFeedback(int recipeId, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var success = await feedbackService.DeleteMyRecipeFeedbackAsync(userId, recipeId, cancellationToken);
        if (!success) return NotFound(new { Message = "Feedback not found." });

        return Ok(new { Message = "Feedback deleted successfully." });
    }

    //  (AI Recipes)

    [HttpGet("ai-recipe/{aiRecipeId}/all")]
    public async Task<IActionResult> GetAiRecipeFeedbacks(int aiRecipeId, CancellationToken cancellationToken)
    {
        var feedbacks = await feedbackService.GetAiRecipeFeedbacksAsync(aiRecipeId, cancellationToken);
        return Ok(feedbacks);
    }

    [HttpGet("ai-recipe/{aiRecipeId}/get-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> GetMyAiRecipeFeedback(int aiRecipeId, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var result = await feedbackService.GetMyAiRecipeFeedbackAsync(userId, aiRecipeId, cancellationToken);
        if (result == null) return NotFound(new { Message = "You haven't rated this AI recipe yet." });

        return Ok(result);
    }

    [HttpPost("ai-recipe/{aiRecipeId}/submit")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> SubmitAiRecipeFeedback(int aiRecipeId, [FromBody] SubmitFeedbackRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        try
        {
            var result = await feedbackService.SubmitAiRecipeFeedbackAsync(userId, aiRecipeId, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("ai-recipe/{aiRecipeId}/delete-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> DeleteMyAiRecipeFeedback(int aiRecipeId, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var success = await feedbackService.DeleteMyAiRecipeFeedbackAsync(userId, aiRecipeId, cancellationToken);
        if (!success) return NotFound(new { Message = "Feedback not found." });

        return Ok(new { Message = "Feedback deleted successfully." });
    }


    [HttpGet("my-history")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> GetAllMyFeedbacks(CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var result = await feedbackService.GetMyFeedbacksAsync(userId, cancellationToken);
        return Ok(result);
    }
}