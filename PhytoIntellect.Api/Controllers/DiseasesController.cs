using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Diseases;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiseasesController(IDiseaseService diseaseService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAllDiseases(CancellationToken cancellationToken)
    {
        var diseases = await diseaseService.GetAllDiseasesAsync(cancellationToken);
        return Ok(diseases);
    }

    [HttpGet("all-names")]
    public async Task<IActionResult> GetDiseasesName(CancellationToken cancellationToken)
    {
        var diseases = await diseaseService.GetDiseasesNameAsync(cancellationToken);
        return Ok(diseases);
    }

    //[HttpPost("add")]
    //[Authorize(Roles = AppRoles.Herbalist)]
    //public async Task<IActionResult> CreateDisease([FromBody] CreateDiseaseRequest request, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var result = await diseaseService.CreateDiseaseAsync(request, cancellationToken);
    //        return Ok(result); // بيرجعلك الـ 200 OK ومعاها بيانات المرض بالـ ID الجديد
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { Message = ex.Message });
    //    }
    //}

}
