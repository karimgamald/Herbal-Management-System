using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.DTOs.PatientDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController(IPatientService patientService) : ControllerBase
{
    // Endpoints خاصة بالمريض

    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var patient = await patientService.GetMyProfileAsync(userId, cancellationToken);
        if (patient == null) return NotFound(new { Message = "Profile not found." });

        return Ok(patient);
    }

    [Authorize(Roles = AppRoles.Patient)]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdatePatientDto request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var result = await patientService.UpdateMyProfileAsync(userId, request, cancellationToken);
        if (result == "Patient profile not found.") return NotFound(new { Message = result });

        return Ok(new { Message = result });
    }

    // Endpoints خاصة بالعشاب / الإدارة

    //[Authorize(Roles = AppRoles.Herbalist)] // العشاب بس اللي يدخل يشوف بروفايل مريض بالـ ID
    //[Authorize(Roles = AppRoles.Patient)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientById(int id, CancellationToken cancellationToken)
    {
        var patient = await patientService.GetPatientByIdAsync(id, cancellationToken);
        if (patient == null) return NotFound(new { Message = "Patient not found." });


        var loggedInUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (patient.UserId != loggedInUserId)
        {
            //return Forbid("You are not authorized to view this patient's data.");
             return BadRequest(new { Message = "Invalid Patient ID requested." });
        }

        return Ok(patient);
    }

    // تقدر تزود Admin هنا قدام لو حبيت
    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllPatients(CancellationToken cancellationToken)
    {
        var patients = await patientService.GetAllPatientsAsync(cancellationToken);
        return Ok(patients);
    }
}