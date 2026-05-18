using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HerbsController(IHerbService herbService) : ControllerBase
{
    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var herbs = await herbService.GetApprovedHerbsAsync(filters, cancellationToken);
        return Ok(herbs);
    }

    [HttpGet("{id}/get-id")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var herb = await herbService.GetHerbByIdAsync(id, cancellationToken);
        if (herb == null) return NotFound("Herb not found.");
        return Ok(herb);
    }

    [HttpGet("{id}/with-herbalist")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdWithHerbalist(int id, CancellationToken cancellationToken)
    {
        var herb = await herbService.GetHerbWithHerbalistAsync(id, cancellationToken);
        if (herb == null) return NotFound("Herb not found.");
        return Ok(herb);
    }

    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("{id}/herbalists")]
    public async Task<IActionResult> GetHerbalistsByHerb(int id, CancellationToken cancellationToken)
    {
        var result = await herbService.GetHerbalistsByHerbIdAsync(id, cancellationToken);
        if (!result.Any()) return NotFound("No herbalists found for this herb.");
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpPost("add")]
    public async Task<IActionResult> Create([FromForm] HerbRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        try
        {
            var result = await herbService.CreateHerbAsync(userId, request, cancellationToken);
            return Ok(new { Message = "Herb proposed successfully and is pending approval.", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpPut("{id}/update")]
    public async Task<IActionResult> Update(int id, [FromForm] HerbRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        try
        {
            var result = await herbService.UpdateHerbAsync(userId, id, request, cancellationToken);
            if (result == null) return NotFound("Pending herb not found.");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpDelete("{id}/delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        try
        {
            var result = await herbService.DeleteHerbAsync(userId, id, cancellationToken);
            if (!result) return NotFound(new { Message = "Pending herb not found." });
            return Ok(new { Message = "Pending herb deleted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    // Admin Endpoints
    [HttpGet("~/api/admin/herbs/pending")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetPendingHerbs([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var herbs = await herbService.GetPendingHerbsAsync(filters, cancellationToken);
        return Ok(herbs);
    }

    [HttpPatch("~/api/admin/herbs/{id}/approve")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> ApproveHerb(int id, CancellationToken cancellationToken)
    {
        var success = await herbService.ApproveHerbAsync(id, cancellationToken);
        if (!success) return NotFound(new { Message = "Herb not found." });
        return Ok(new { Message = "Herb approved successfully." });
    }

    [HttpPost("~/api/admin/herbs/add")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> AdminCreate([FromForm] HerbRequest request, CancellationToken cancellationToken)
    {
        var result = await herbService.AdminCreateHerbAsync(request, cancellationToken);
        return Ok(new { Message = "Herb added and approved directly.", Data = result });
    }

    [HttpPut("~/api/admin/herbs/{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> AdminUpdate(int id, [FromForm] HerbRequest request, CancellationToken cancellationToken)
    {
        var result = await herbService.AdminUpdateHerbAsync(id, request, cancellationToken);
        if (result == null) return NotFound("Herb not found.");
        return Ok(result);
    }

    [HttpDelete("~/api/admin/herbs/{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> AdminDelete(int id, CancellationToken cancellationToken)
    {
        var result = await herbService.AdminDeleteHerbAsync(id, cancellationToken);
        if (!result) return NotFound(new { Message = "Herb not found." });
        return Ok(new { Message = "Herb deleted successfully by Admin." });
    }
}