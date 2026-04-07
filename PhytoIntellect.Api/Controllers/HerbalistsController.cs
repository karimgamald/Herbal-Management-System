using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.DTOs.HerbalistDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HerbalistsController(IHerbalistService herbalistService) : ControllerBase
{
    // ===============================
    // Endpoints خاصة بالعشاب نفسه
    // ===============================
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Herbalist}")]
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

    // ===============================
    // Endpoints للإدارة / العرض العام
    // ===============================

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Herbalist}")]
    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetHerbalistById(int id, CancellationToken cancellationToken)
    {
        var herbalist = await herbalistService.GetHerbalistByIdAsync(id, cancellationToken);

        if (herbalist == null)
            return NotFound(new { Message = "Herbalist not found." });

        return Ok(herbalist);
    }

    [Authorize(Roles = $"{AppRoles.Patient}")]
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllHerbalists(CancellationToken cancellationToken)
    {
        var herbalists = await herbalistService.GetAllHerbalistsAsync(cancellationToken);
        return Ok(herbalists);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Herbalist}")]
    [HttpPut("update-profile/me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] CreateOrUpdateHerbalistDto request,
        CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var result = await herbalistService.UpdateMyProfileAsync(userId, request, cancellationToken);

        if (result == "Herbalist profile not found.")
            return NotFound(new { Message = result });

        return Ok(new { Message = result });
    }
}