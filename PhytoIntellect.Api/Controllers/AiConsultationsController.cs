using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Interfaces;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AiConsultationsController : ControllerBase
{
    private readonly IAiRecipeService _aiRecipeService;
    private readonly IValidator<CreateAiRecipeRequest> _validator; // 👈 1. حقن الـ Validator
    private readonly ILogger<AiConsultationsController> _logger;   // 👈 2. حقن الـ Logger

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
        // 1. الـ Validation باستخدام הـ Injected Validator
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            // بنرجع الـ Errors بشكل منسق للفرونت إند
            return BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            // 2. تنفيذ البيزنس لوجيك
            var response = await _aiRecipeService.GenerateRecipeAsync(request);

            // 3. إرجاع النتيجة
            return Ok(response);
        }
        catch (Exception ex)
        {
            // 4. تسجيل الإيرور بالتفصيل عندنا إحنا في السيرفر
            _logger.LogError(ex, "Error occurred while generating AI recipe for PatientId: {PatientId}", request.PatientId);

            // 5. إرجاع رسالة آمنة للموبايل
            return StatusCode(500, new { message = "An error occurred while communicating with the AI Engine. Please try again later." });
        }
    }
    //private readonly IAiRecipeService _aiRecipeService;

    //public AiConsultationsController(IAiRecipeService aiRecipeService)
    //{
    //    _aiRecipeService = aiRecipeService;
    //}

    //[HttpPost("generate")]
    //public async Task<IActionResult> GenerateRecipe([FromBody] CreateAiRecipeRequest request)
    //{
    //    // 1. الـ Validation (لو مش عامل Auto-Validation في الـ Program.cs)
    //    var validator = new CreateAiRecipeValidator();
    //    var validationResult = await validator.ValidateAsync(request);

    //    if (!validationResult.IsValid)
    //    {
    //        return BadRequest(validationResult.Errors);
    //    }

    //    try
    //    {
    //        // 2. تنفيذ البيزنس لوجيك
    //        var response = await _aiRecipeService.GenerateRecipeAsync(request);

    //        // 3. إرجاع النتيجة
    //        return Ok(response);
    //    }
    //    catch (Exception ex)
    //    {
    //        // يُفضل استخدام Logger هنا بدل الـ Console
    //        return StatusCode(500, new { message = "An error occurred while communicating with the AI Engine.", details = ex.Message });
    //    }
    //}
}
