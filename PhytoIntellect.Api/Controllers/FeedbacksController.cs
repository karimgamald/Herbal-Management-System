using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Feedbacks;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;
using System;
using System.Threading;
using System.Threading.Tasks;
using PhytoIntellect.Application.Paginations;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class FeedbacksController(IFeedbackService feedbackService) : ControllerBase
{
    // (Regular Recipes)
    [HttpGet("recipe/{id}/all")]
    public async Task<IActionResult> GetRecipeFeedbacks(int id, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var feedbacks = await feedbackService.GetRecipeFeedbacksAsync(id, filters, cancellationToken);
        return Ok(feedbacks);
    }

    [HttpGet("recipe/{id}/get-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> GetMyRecipeFeedback(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var result = await feedbackService.GetMyRecipeFeedbackAsync(userId, id, cancellationToken);
        if (result == null) 
            return NotFound(new { Message = "You haven't rated this recipe yet." });

        return Ok(result);
    }

    [HttpPost("recipe/{id}/submit")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> SubmitRecipeFeedback(int id, [FromBody] SubmitFeedbackRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) 
            return Unauthorized();

        try
        {
            var result = await feedbackService.SubmitRecipeFeedbackAsync(userId, id, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("recipe/{id}/delete-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> DeleteMyRecipeFeedback(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) 
            return Unauthorized();

        var success = await feedbackService.DeleteMyRecipeFeedbackAsync(userId, id, cancellationToken);
        if (!success) return NotFound(new { Message = "Feedback not found." });

        return Ok(new { Message = "Feedback deleted successfully." });
    }

    //  (AI Recipes)

    [HttpGet("ai-recipe/{id}/all")]
    public async Task<IActionResult> GetAiRecipeFeedbacks(int id, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var feedbacks = await feedbackService.GetAiRecipeFeedbacksAsync(id, filters, cancellationToken);
        return Ok(feedbacks);
    }

    [HttpGet("ai-recipe/{id}/get-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> GetMyAiRecipeFeedback(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var result = await feedbackService.GetMyAiRecipeFeedbackAsync(userId, id, cancellationToken);
        if (result == null)
            return NotFound(new { Message = "You haven't rated this AI recipe yet." });

        return Ok(result);
    }

    [HttpPost("ai-recipe/{id}/submit")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> SubmitAiRecipeFeedback(int id, [FromBody] SubmitFeedbackRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) 
            return Unauthorized();

        try
        {
            var result = await feedbackService.SubmitAiRecipeFeedbackAsync(userId, id, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("ai-recipe/{id}/delete-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> DeleteMyAiRecipeFeedback(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) 
            return Unauthorized();

        var success = await feedbackService.DeleteMyAiRecipeFeedbackAsync(userId, id, cancellationToken);
        if (!success)
            return NotFound(new { Message = "Feedback not found." });

        return Ok(new { Message = "Feedback deleted successfully." });
    }


    [HttpGet("ai-chat-recipe/{id}/all")]
    public async Task<IActionResult> GetAiChatRecipeFeedbacks(int id, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var feedbacks = await feedbackService.GetAiChatRecipeFeedbacksAsync(id, filters, cancellationToken);
        return Ok(feedbacks);
    }

    [HttpGet("ai-chat-recipe/{id}/get-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> GetMyAiChatRecipeFeedback(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var result = await feedbackService.GetMyAiChatRecipeFeedbackAsync(userId, id, cancellationToken);
        if (result == null)
            return NotFound(new { Message = "You haven't rated this AI chat recipe yet." });

        return Ok(result);
    }

    [HttpPost("ai-chat-recipe/{id}/submit")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> SubmitAiChatRecipeFeedback(int id, [FromBody] SubmitFeedbackRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        try
        {
            var result = await feedbackService.SubmitAiChatRecipeFeedbackAsync(userId, id, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("ai-chat-recipe/{id}/delete-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> DeleteMyAiChatRecipeFeedback(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var success = await feedbackService.DeleteMyAiChatRecipeFeedbackAsync(userId, id, cancellationToken);
        if (!success)
            return NotFound(new { Message = "Feedback not found." });

        return Ok(new { Message = "Feedback deleted successfully." });
    } 


    [HttpGet("my-history")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> GetAllMyFeedbacks([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) 
            return Unauthorized();

        var result = await feedbackService.GetMyFeedbacksAsync(userId, filters, cancellationToken);
        return Ok(result);
    }


    // Admin Endpoints
    [HttpGet("~/api/admin/feedbacks")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> AdminGetAllFeedbacks([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var feedbacks = await feedbackService.AdminGetAllFeedbacksAsync(filters, cancellationToken);
        return Ok(feedbacks);
    }

    [HttpDelete("~/api/admin/feedbacks/{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> AdminDeleteFeedback(int id, CancellationToken cancellationToken)
    {
        var success = await feedbackService.AdminDeleteFeedbackAsync(id, cancellationToken);
        if (!success) return NotFound(new { Message = "Feedback not found." });

        return Ok(new { Message = "Feedback deleted successfully by Admin." });
    }
}