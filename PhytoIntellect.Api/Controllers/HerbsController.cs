using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Herbs;
using System.Security.Claims;

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

    [HttpPost("add")]
    public async Task<IActionResult> Create([FromForm] HerbRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var herbalistId = int.Parse(userIdClaim.Value);

        var result = await herbService.CreateHerbAsync(herbalistId, request, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id}/update")]
    public async Task<IActionResult> Update(int id, [FromForm] HerbRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var herbalistId = int.Parse(userIdClaim.Value);

        var result = await herbService.UpdateHerbAsync(herbalistId, id, request, cancellationToken);

        if (result == null)
            return NotFound("Herb not found.");

        return Ok(result);
    }

    [HttpDelete("{id}/delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await herbService.DeleteHerbAsync(id, cancellationToken);

        if (!result)
            return NotFound(new { Message = "Herb not found." });

        return Ok(new { Message = "Herb deleted successfully." });
    }
}