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

    [HttpDelete("{id}/delete")]
    public async Task<IActionResult> DeleteNotification(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var success = await notificationService.DeleteNotificationAsync(id, int.Parse(userIdStr), cancellationToken);
        if (!success) return NotFound(new { Message = "Notification not found." });

        return Ok(new { Message = "Notification deleted successfully." });
    }

    [HttpPatch("mark-all-as-read")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var success = await notificationService.MarkAllAsReadAsync(int.Parse(userIdStr), cancellationToken);
        if (!success) return Ok(new { Message = "No unread notifications found." });

        return Ok(new { Message = "All notifications marked as read successfully." });
    }
}