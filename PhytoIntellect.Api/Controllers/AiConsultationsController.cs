using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Interfaces;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AiConsultationsController : ControllerBase
{
    private readonly IAiRecipeService _aiRecipeService;
    private readonly IValidator<CreateAiRecipeRequest> _validator;
    private readonly ILogger<AiConsultationsController> _logger;

    public AiConsultationsController(
        IAiRecipeService aiRecipeService,
        IValidator<CreateAiRecipeRequest> validator,
        ILogger<AiConsultationsController> logger)
    {
        _aiRecipeService = aiRecipeService;
        _validator = validator;
        _logger = logger;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateRecipe([FromBody] CreateAiRecipeRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);

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

            var response = await _aiRecipeService.GenerateRecipeAsync(userIdInt, request);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "User attempted to generate a recipe without a patient profile.");
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
            _logger.LogError(ex, "Error occurred while generating AI recipe for UserId: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            return StatusCode(500, new { Message = "An internal error occurred while communicating with the AI Engine. Please try again later." });
        }
    }
}
