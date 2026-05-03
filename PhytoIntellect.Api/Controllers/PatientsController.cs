using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Patients;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController(IPatientService patientService) : ControllerBase
{

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


    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientById(int id, CancellationToken cancellationToken)
    {
        var patient = await patientService.GetPatientByIdAsync(id, cancellationToken);
        if (patient == null) return NotFound(new { Message = "Patient not found." });


        var loggedInUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (patient.UserId != loggedInUserId)
        {
            return BadRequest(new { Message = "Invalid Patient ID requested." });
        }

        return Ok(patient);
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllPatients([FromQuery] RequestFilters filters ,CancellationToken cancellationToken)
    {
        var patients = await patientService.GetAllPatientsAsync(filters,cancellationToken);
        return Ok(patients);
    }

    [Authorize(Roles = AppRoles.Patient)]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdatePatientRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var result = await patientService.UpdateMyProfileAsync(userId, request, cancellationToken);
        if (result == "Patient profile not found.") return NotFound(new { Message = result });

        return Ok(new { Message = result });
    }
}