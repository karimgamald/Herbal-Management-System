using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.DTOs.MedicalHistoryDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
// شيلنا الـ Authorize من هنا عشان نوزع الصلاحيات جوه
public class MedicalHistoriesController(IMedicalHistoryService medicalHistoryService) : ControllerBase
{
    // 1. للمريض بس
    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyMedicalHistory(CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var history = await medicalHistoryService.GetMyMedicalHistoryAsync(userId, cancellationToken);
        if (history == null) return NotFound(new { Message = "Medical history not found." });

        return Ok(history);
    }

    // 2. للمريض بس
    [Authorize(Roles = AppRoles.Patient)]
    [HttpPost("me")]
    public async Task<IActionResult> ManageMyMedicalHistory([FromBody] ManageMedicalHistoryDto request,
        CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var result = await medicalHistoryService.AddOrUpdateMyMedicalHistoryAsync(userId, request,
            cancellationToken);
        if (result.Contains("not found") || result.Contains("Error")) return BadRequest(new { Message = result });

        return Ok(new { Message = result });
    }

    // 3. ✨ دي الإضافة الجديدة: للعشاب بس!
    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientMedicalHistory(int patientId, CancellationToken cancellationToken)
    {
        var history = await medicalHistoryService.GetPatientMedicalHistoryByPatientIdAsync(patientId,
            cancellationToken);

        // لو المريض معندوش تاريخ مرضي، بنبلغ العشاب عشان ياخد باله
        if (history == null) return NotFound(new { Message = "This patient has not provided a medical history yet." });

        return Ok(history);
    }
}