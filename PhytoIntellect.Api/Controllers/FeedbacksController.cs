using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Feedbacks;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants; 
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/recipe/{recipeId}/[controller]")]
public class FeedbacksController(IFeedbackService feedbackService) : ControllerBase
{
    // 2️⃣ جلب كل التقييمات لوصفة معينة (متاحة للكل)
    [HttpGet("all")]
    public async Task<IActionResult> GetRecipeFeedbacks(int recipeId, CancellationToken cancellationToken)
    {
        var feedbacks = await feedbackService.GetRecipeFeedbacksAsync(recipeId, cancellationToken);
        return Ok(feedbacks);
    }

    // 3️⃣ جلب تقييم المريض الحالي للوصفة (عشان الفلاتر ينور النجوم)
    [HttpGet("get-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> GetMyFeedback(int recipeId, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var result = await feedbackService.GetMyFeedbackAsync(userId, recipeId, cancellationToken);
        if (result == null) return NotFound(new { Message = "You haven't rated this recipe yet." });

        return Ok(result);
    }

    // 1️⃣ إضافة أو تعديل تقييم
    [HttpPost("submit")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> SubmitFeedback(int recipeId, [FromBody] SubmitFeedbackRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        try
        {
            // ✅ بص هنا: باصينا الـ recipeId لوحده، والـ request لوحده للـ Service
            var result = await feedbackService.SubmitFeedbackAsync(userId, recipeId, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    // 4️⃣ حذف التقييم
    [HttpDelete("delete-me")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> DeleteMyFeedback(int recipeId, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var success = await feedbackService.DeleteMyFeedbackAsync(userId, recipeId, cancellationToken);
        if (!success) return NotFound(new { Message = "Feedback not found." });

        return Ok(new { Message = "Feedback deleted successfully." });
    }
}