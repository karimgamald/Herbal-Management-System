using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Diseases;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiseasesController(IDiseaseService diseaseService) : ControllerBase
{
    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllDiseases([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var diseases = await diseaseService.GetAllDiseasesAsync(filters, cancellationToken);
        return Ok(diseases);
    }

    [HttpGet("all-names")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDiseasesName(CancellationToken cancellationToken)
    {
        var diseases = await diseaseService.GetDiseasesNameAsync(cancellationToken);
        return Ok(diseases);
    }

    [HttpPost("propose")] 
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> ProposeDisease([FromBody] CreateDiseaseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await diseaseService.CreateDiseaseAsync(request, cancellationToken);
            return Ok(new { Message = "Disease proposed successfully and is pending admin approval.", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    // Admin Endpoints
    [HttpGet("~/api/admin/diseases/pending")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetPendingDiseases([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var diseases = await diseaseService.GetPendingDiseasesAsync(filters, cancellationToken);
        return Ok(diseases);
    }

    [HttpPost("~/api/admin/diseases/add")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> AddDiseaseByAdmin([FromBody] CreateDiseaseRequest request, [FromQuery] bool isAiSupported, CancellationToken cancellationToken)
    {
        try
        {
            var result = await diseaseService.AddDiseaseByAdminAsync(request, isAiSupported, cancellationToken);
            return Ok(new { Message = "Disease added and approved automatically.", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPatch("~/api/admin/diseases/{id}/approve")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> ApproveDisease(int id, CancellationToken cancellationToken)
    {
        var success = await diseaseService.ApproveDiseaseAsync(id, cancellationToken);
        if (!success) return NotFound(new { Message = "Disease not found." });

        return Ok(new { Message = "Disease approved successfully." });
    }

    [HttpDelete("~/api/admin/diseases/{id}/reject")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> RejectDisease(int id, CancellationToken cancellationToken)
    {
        var success = await diseaseService.RejectDiseaseAsync(id, cancellationToken);
        if (!success) return NotFound(new { Message = "Disease not found." });

        return Ok(new { Message = "Disease rejected and removed successfully." });
    }
}