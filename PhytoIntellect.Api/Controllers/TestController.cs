using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Core.Constants; // عشان يشوف كلاس الـ AppRoles اللي عملناه
using System.Security.Claims; // عشان نقرا البيانات من التوكن

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    // 1. Endpoint عامة (أي حد معاه توكن سليم يدخل)
    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        return Ok(new { Message = "Access Granted: You are an authenticated user!" });
    }

    // 2. Endpoint للـ Herbalist بس (لو Patient حاول يدخل هياخد 403 Forbidden)
    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpGet("herbalist-only")]
    public IActionResult HerbalistEndpoint()
    {
        return Ok(new { Message = "Access Granted: Welcome Herbalist!" });
    }

    // 3. Endpoint للـ Patient بس (لو Herbalist حاول يدخل هياخد 403 Forbidden)
    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("patient-only")]
    public IActionResult PatientEndpoint()
    {
        return Ok(new { Message = "Access Granted: Welcome Patient!" });
    }

    // 4. Endpoint مشتركة (للاتنين مع بعض)
    [Authorize(Roles = $"{AppRoles.Patient},{AppRoles.Herbalist}")]
    [HttpGet("shared")]
    public IActionResult SharedEndpoint()
    {
        return Ok(new { Message = "Access Granted: Both Patients and Herbalists are allowed here." });
    }

    // 5. Endpoint سحرية (بتقرا بياناتك من التوكن نفسه من غير ما تكلم الداتابيز!)
    [Authorize]
    [HttpGet("my-data")]
    public IActionResult GetMyDataFromToken()
    {
        // بنسحب البيانات اللي حطيناها في الـ TokenService زمان
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            Message = "Here is the data extracted directly from your JWT Token:",
            UserId = userId,
            UserName = userName,
            Role = userRole
        });
    }
}