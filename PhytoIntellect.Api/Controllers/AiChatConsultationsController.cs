using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Api.Extensions;
using PhytoIntellect.Application.Contracts.ChatAiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[Route("api/AiChat")]
[ApiController]
public class AiChatConsultationsController(
    IChatAiRecipeService chatAiRecipeService,
    ILogger<AiChatConsultationsController> logger) : ControllerBase
{
    [HttpPost("chat-generate")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> GenerateChatRecipe([FromBody] CreateChatRecipeRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserPrompt))
        {
            return BadRequest(new { Message = "Please describe your symptoms." });
        }

        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userIdInt))
            {
                return Unauthorized(new { Message = "Invalid authentication token." });
            }

            var response = await chatAiRecipeService.GenerateChatRecipeAsync(userIdInt, request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating Chat AI recipe.");
            return StatusCode(500, new { Message = "Internal Server Error", Details = ex.Message });
        }
    }
}