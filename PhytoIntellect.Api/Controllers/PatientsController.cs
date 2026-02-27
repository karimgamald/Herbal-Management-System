using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.DTOs.PatientDTOs;
using PhytoIntellect.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController(IPatientService patientService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto request, CancellationToken cancellationToken)
    {
        var result = await patientService.CreatePatientAsync(request, cancellationToken);
        return Ok(new { Message = result });
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllPatients(CancellationToken cancellationToken)
    {
        var patients = await patientService.GetAllPatientsAsync(cancellationToken);
        return Ok(patients);
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetPatientById(int id, CancellationToken cancellationToken)
    {
        var patient = await patientService.GetPatientByIdAsync(id, cancellationToken);
        if (patient == null) return NotFound(new { Message = "Patient not found." });

        return Ok(patient);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientDto request, CancellationToken cancellationToken)
    {
        var result = await patientService.UpdatePatientAsync(id, request, cancellationToken);
        if (result == "Patient not found.") return NotFound(new { Message = result });

        return Ok(new { Message = result });
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeletePatient(int id, CancellationToken cancellationToken)
    {
        var result = await patientService.DeletePatientAsync(id, cancellationToken);
        if (result == "Patient not found.") return NotFound(new { Message = result });

        return Ok(new { Message = result });
    }
}