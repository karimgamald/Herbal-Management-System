using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Interfaces;

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
}
