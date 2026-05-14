using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Api.Extensions;
using PhytoIntellect.Application.Contracts.ChatAiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[Route("api/AiChat")]
[ApiController]
public class AiChatConsultationsController(IChatAiRecipeService chatAiRecipeService,
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

    [HttpGet("catalog")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPublicRecipes([FromQuery] RequestFilters filters,
       CancellationToken cancellationToken)
    {
        try
        {
            var recipes = await chatAiRecipeService
                .GetAllPublicAsync(filters, cancellationToken);

            if (recipes == null || !recipes.Items.Any())
            {
                return BadRequest(new
                {
                    Message = "No AI chat recipes added to the system yet."
                });
            }

            return Ok(recipes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error retrieving public AI chat recipes");

            return StatusCode(500, new
            {
                Message = "Internal server error"
            });
        }
    }

    // =========================================
    // Get Public Recipe By Id
    // =========================================
    [HttpGet("{id}/catalog")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicById(int id,CancellationToken cancellationToken)
    {
        try
        {
            var result = await chatAiRecipeService
                .GetPublicByIdAsync(id, cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error retrieving public AI chat recipe");

            return StatusCode(500, new
            {
                Message = "Internal server error"
            });
        }
    }

    // =========================================
    // Get All Patient Recipes
    // =========================================
    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("myConsultations")]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilters filters,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) ||
                !int.TryParse(userIdString, out int userIdInt))
            {
                return Unauthorized(new
                {
                    Message = "Invalid or missing authentication token."
                });
            }

            var result = await chatAiRecipeService
                .GetPatientAllAsync(userIdInt, filters, cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error retrieving AI chat recipes");

            return StatusCode(500, new
            {
                Message = "Internal server error"
            });
        }
    }

    // =========================================
    // Get Patient Recipe By Id
    // =========================================
    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("{id}/myConsultation")]
    public async Task<IActionResult> GetById(int id,CancellationToken cancellationToken)
    {
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) ||
                !int.TryParse(userIdString, out int userIdInt))
            {
                return Unauthorized(new
                {
                    Message = "Invalid or missing authentication token."
                });
            }

            var result = await chatAiRecipeService
                .GetPatientRecipeByIdAsync(userIdInt, id, cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return NotFound(new
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error retrieving AI chat recipe {RecipeId}", id);

            return StatusCode(500, new
            {
                Message = "Internal server error"
            });
        }
    }

}