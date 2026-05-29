using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Notifications;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("my-notifications")]
    [Authorize(Roles = $"{AppRoles.Patient} , {AppRoles.Herbalist}")]
    public async Task<IActionResult> GetMyNotifications([FromQuery] bool? isRead = null, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var notis = await notificationService.GetMyNotificationsAsync(userId, isRead, cancellationToken);
        return Ok(notis);
    }

    [HttpPatch("{id}/mark-as-read")]
    [Authorize(Roles = $"{AppRoles.Patient} , {AppRoles.Herbalist}")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var success = await notificationService.MarkAsReadAsync(id, userId, cancellationToken);

        if (!success) return NotFound(new { Message = "Notification not found or already read." });
        return Ok(new { Message = "Marked as read successfully." });
    }

    [HttpPatch("mark-all-as-read")]
    [Authorize(Roles = $"{AppRoles.Patient} , {AppRoles.Herbalist}")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var success = await notificationService.MarkAllAsReadAsync(int.Parse(userIdStr), cancellationToken);
        if (!success) return Ok(new { Message = "No unread notifications found." });

        return Ok(new { Message = "All notifications marked as read successfully." });
    }

    [HttpDelete("{id}/delete")]
    [Authorize(Roles = $"{AppRoles.Patient} , {AppRoles.Herbalist}")]
    public async Task<IActionResult> DeleteNotification(int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var success = await notificationService.DeleteNotificationAsync(id, int.Parse(userIdStr), cancellationToken);
        if (!success) return NotFound(new { Message = "Notification not found." });

        return Ok(new { Message = "Notification deleted successfully." });
    }

    // ================= Admin Endpoints =================
    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("~/api/admin/notifications/send-bulk")]
    public async Task<IActionResult> SendBulkNotification([FromBody] AdminNotificationRequest request, CancellationToken cancellationToken)
    {
        if (request.TargetRole != AppRoles.Herbalist && request.TargetRole != AppRoles.Patient && request.TargetRole.ToLower() != "all")
        {
            return BadRequest(new { Message = "Invalid Target Role. Use 'Herbalist', 'Patient', or 'All'." });
        }

        await notificationService.SendBulkNotificationAsync(request, cancellationToken);
        return Ok(new { Message = $"Notification sent successfully to {request.TargetRole}." });
    }
}