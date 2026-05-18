using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Herbalists;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HerbalistsController(IHerbalistService herbalistService) : ControllerBase
{
    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpGet("get-profile/me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var herbalist = await herbalistService.GetMyProfileAsync(userId, cancellationToken);

        if (herbalist == null)
            return NotFound(new { Message = "Profile not found." });

        return Ok(herbalist);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Herbalist},{AppRoles.Patient}")]
    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetHerbalistById(int id, CancellationToken cancellationToken)
    {
        var herbalist = await herbalistService.GetHerbalistByIdAsync(id, cancellationToken);

        if (herbalist == null)
            return NotFound(new { Message = "Herbalist not found." });

        return Ok(herbalist);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Herbalist},{AppRoles.Patient}")]
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllHerbalists([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var herbalists = await herbalistService.GetAllHerbalistsAsync(filters, cancellationToken);
        return Ok(herbalists);
    }

    [Authorize(Roles = AppRoles.Herbalist)]
    [HttpPut("update-profile/me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] CreateOrUpdateHerbalistRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var result = await herbalistService.UpdateMyProfileAsync(userId, request, cancellationToken);

        if (result == "Herbalist profile not found.")
            return NotFound(new { Message = result });

        return Ok(new { Message = result });
    }

    // Admin Endpoints
    [HttpGet("~/api/admin/herbalists")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAllHerbalistsByAdmin([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var herbalists = await herbalistService.GetAllHerbalistsByAdminAsync(filters, cancellationToken);
        return Ok(herbalists);
    }

    [HttpDelete("~/api/admin/herbalists/{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> DeleteHerbalistByAdmin(int id, CancellationToken cancellationToken)
    {
        var success = await herbalistService.DeleteHerbalistAsync(id, cancellationToken);
        if (!success)
            return NotFound(new { Message = "Herbalist not found." });

        return Ok(new { Message = "Herbalist deleted successfully by Admin." });
    }

    [HttpGet("~/api/admin/herbalists/stats")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetHerbalistsStats(CancellationToken cancellationToken)
    {
        var stats = await herbalistService.GetHerbalistsStatsAsync(cancellationToken);
        return Ok(stats);
    }
}