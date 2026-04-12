using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.Services;
using PhytoIntellect.Core.Constants;
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

    [HttpGet("{id}/with-herbalist")]
    public async Task<IActionResult> GetByIdWithHerbalist(int id, CancellationToken cancellationToken)
    {
        var herb = await herbService.GetHerbWithHerbalistAsync(id, cancellationToken);

        if (herb == null)
            return NotFound("Herb not found.");

        return Ok(herb);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Patient}")]
    [HttpGet("{id}/herbalists")]
    public async Task<IActionResult> GetHerbalistsByHerb(int id, CancellationToken cancellationToken)
    {
        var result = await herbService.GetHerbalistsByHerbIdAsync(id, cancellationToken);

        if (!result.Any())
            return NotFound("No herbalists found for this herb.");

        return Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Herbalist}")]
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

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Herbalist}")]
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

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Herbalist}")]
    [HttpDelete("{id}/delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await herbService.DeleteHerbAsync(id, cancellationToken);

        if (!result)
            return NotFound(new { Message = "Herb not found." });

        return Ok(new { Message = "Herb deleted successfully." });
    }
}