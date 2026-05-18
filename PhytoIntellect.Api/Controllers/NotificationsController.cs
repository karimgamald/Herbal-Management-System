using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Interfaces;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    [HttpGet("my-notifications")]
    public async Task<IActionResult> GetMyNotifications([FromQuery] bool? isRead = null, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var notis = await notificationService.GetMyNotificationsAsync(userId, isRead, cancellationToken);
        return Ok(notis);
    }

    [HttpPatch("{id}/mark-as-read")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var success = await notificationService.MarkAsReadAsync(id, userId, cancellationToken);

        if (!success) return NotFound(new { Message = "Notification not found or already read." });
        return Ok(new { Message = "Marked as read successfully." });
    }
}