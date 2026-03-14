
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Core.Entities;

[ApiController]
[Route("api/[controller]")]
public class HerbsController(IHerbService herbService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var herbs = await herbService.GetApprovedHerbsAsync(cancellationToken);
        return Ok(herbs);
    }

    [HttpGet("{id}/get-id")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var herb = await herbService.GetHerbByIdAsync(id, cancellationToken);

        if (herb == null)
            return NotFound("Herb not found.");

        return Ok(herb);
    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] HerbRequest request, CancellationToken cancellationToken)
    {
        // 🔹 Get herbalistId from claims
        var herbalistIdClaim = User.FindFirst("herbalistId")?.Value;
        if (herbalistIdClaim == null)
            return Unauthorized("Herbalist not found in token.");

        int herbalistId = int.Parse(herbalistIdClaim);

        var result = await herbService.CreateHerbAsync(herbalistId, request, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] HerbRequest request, CancellationToken cancellationToken)
    {
        // 🔹 Get herbalistId from claims
        var herbalistIdClaim = User.FindFirst("herbalistId")?.Value;
        if (herbalistIdClaim == null)
            return Unauthorized("Herbalist not found in token.");

        int herbalistId = int.Parse(herbalistIdClaim);

        var result = await herbService.UpdateHerbAsync(herbalistId, id, request, cancellationToken);

        if (result == null)
            return NotFound("Herb not found.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("{id}/delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await herbService.DeleteHerbAsync(id, cancellationToken);
        if (!result) return NotFound(new { Message = "Herb not found." });

        return Ok(new { Message = "Herb deleted successfully." });
    }
}