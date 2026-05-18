using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Infrastructure.UOW;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AiConsultationsController(
        IAiRecipeService aiRecipeService,
        IValidator<CreateAiRecipeRequest> validator,
        ILogger<AiConsultationsController> logger) : ControllerBase
{
    [HttpGet("catalog")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPublicRecipes([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        try
        {
            var recipes = await aiRecipeService.GetAllPublicAsync(filters, cancellationToken);
            if (recipes == null || !recipes.Items.Any())
            {
                return BadRequest(new
                {
                    Message = "No AI recipes added to the system yet."
                });
            }

            return Ok(recipes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving public AI recipes");

            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpGet("{id}/catalog")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await aiRecipeService.GetPublicByIdAsync(id, cancellationToken);
            if (result == null)
            {
                return BadRequest(new
                {
                    Message = "No AI recipes added to the system yet."
                });
            }
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("myConsultations")]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userIdInt))
                return Unauthorized(new { Message = "Invalid or missing authentication token." });

            var result = await aiRecipeService.GetAllAsync(userIdInt, filters, cancellationToken);

            if (result == null || !result.Items.Any())
            {
                return Ok(result);
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound(new { Message = "Recipe not found or access denied." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving AI recipes for UserId: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("{id}/myConsultation")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userIdInt))
                return Unauthorized(new { Message = "Invalid or missing authentication token." });

            var result = await aiRecipeService.GetByIdAsync(userIdInt, id, cancellationToken);
            if (result == null)
            {
                return BadRequest(new
                {
                    Message = "No AI recipes found for this user."
                });
            }

            return Ok(result);

        }
        catch (UnauthorizedAccessException)
        {
            return NotFound(new { Message = "Recipe not found or access denied." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving AI recipe {RecipeId}", id);

            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpPost("generate")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> GenerateRecipe([FromBody] CreateAiRecipeRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userIdInt))
            {
                return Unauthorized(new { Message = "Invalid or missing authentication token." });
            }

            var response = await aiRecipeService.GenerateRecipeAsync(userIdInt, request);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "User attempted to generate a recipe without a patient profile.");
            return Unauthorized(new { Message = "Patient profile not found for this user. Please complete your profile registration." });
        }
        catch (InvalidOperationException ex) when (ex.Message == "PROFILE_INCOMPLETE_DOB" || ex.Message == "PROFILE_INCOMPLETE_GENDER")
        {
            return BadRequest(new
            {
                ErrorCode = "PROFILE_INCOMPLETE",
                Message = "Please complete your basic profile information (Gender and Date of Birth) before requesting a consultation."
            });
        }
        catch (InvalidOperationException ex) when (ex.Message == "MEDICAL_HISTORY_MISSING")
        {
            return BadRequest(new
            {
                ErrorCode = "MEDICAL_HISTORY_MISSING",
                Message = "Please complete your medical history profile before requesting an AI consultation."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while generating AI recipe for UserId: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            return StatusCode(500, new { Message = "An internal error occurred while communicating with the AI Engine. Please try again later." });
        }
    }


    // [Admin] Get All Global Consultations
    [HttpGet("~/api/admin/ai-consultations")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAllConsultationsByAdmin([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        try
        {
            var result = await aiRecipeService.GetAllSystemConsultationsAsync(filters, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all AI consultations for Admin.");
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpGet("~/api/admin/ai-consultations/statistics")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAiStatisticsByAdmin(CancellationToken cancellationToken)
    {
        try
        {
            var stats = await aiRecipeService.GetAiModelStatisticsAsync(cancellationToken);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving AI model statistics for Admin.");
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpDelete("~/api/admin/ai-consultations/{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> DeleteAiRecipeByAdmin(int id, CancellationToken cancellationToken)
    {
        try
        {
            var success = await aiRecipeService.DeleteAsync(id, cancellationToken);
            if (!success)
                return NotFound(new { Message = "AI Recipe not found." });

            return Ok(new { Message = "AI Recipe deleted successfully by Admin." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting AI recipe {RecipeId} by Admin", id);
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }
}
