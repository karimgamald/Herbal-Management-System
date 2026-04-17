using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.MedicalHistories;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicalHistoriesController(IMedicalHistoryService medicalHistoryService) : ControllerBase
{
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

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientMedicalHistory(int patientId, CancellationToken cancellationToken)
    {
        var history = await medicalHistoryService.GetPatientMedicalHistoryByPatientIdAsync(patientId, cancellationToken);

        if (history == null) return NotFound(new { Message = "This patient has not provided a medical history yet." });

        return Ok(history);
    }

    [Authorize(Roles = AppRoles.Patient)]
    [HttpPost("me")]
    public async Task<IActionResult> ManageMyMedicalHistory([FromBody] MedicalHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var result = await medicalHistoryService.AddOrUpdateMyMedicalHistoryAsync(userId, request,
            cancellationToken);
        if (result.Contains("not found") || result.Contains("Error")) return BadRequest(new { Message = result });

        return Ok(new { Message = result });
    }
}