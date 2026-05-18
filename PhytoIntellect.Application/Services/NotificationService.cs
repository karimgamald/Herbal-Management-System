using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Services;

public class NotificationService(IUnitOfWork unitOfWork) : INotificationService
{
    public async Task SendNotificationAsync(int userId, string title, string message, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.NotificationRepository.CreateAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<object>> GetMyNotificationsAsync(int userId, bool? isRead = false, CancellationToken cancellationToken = default)
    {
        var notifications = await unitOfWork.NotificationRepository.GetAllAsync(
            filter: n => n.UserId == userId && (!isRead.HasValue || n.IsRead == isRead.Value),
            tracked: false,
            cancellationToken: cancellationToken);

        return notifications.OrderByDescending(n => n.CreatedAt)
            .Select(n => new
            {
                n.Id,
                n.Title,
                n.Message,
                n.IsRead,
                n.CreatedAt
            });
    } 

    public async Task<bool> MarkAsReadAsync(int notificationId, int userId, CancellationToken cancellationToken = default)
    {
        var notification = await unitOfWork.NotificationRepository.GetAsync(
            filter: n => n.Id == notificationId && n.UserId == userId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (notification == null) return false;

        notification.IsRead = true;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteNotificationAsync(int notificationId, int userId, CancellationToken cancellationToken = default)
    {
        var notification = await unitOfWork.NotificationRepository.GetAsync(
            filter: n => n.Id == notificationId && n.UserId == userId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (notification == null) return false;

        unitOfWork.NotificationRepository.Remove(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        var notifications = await unitOfWork.NotificationRepository.GetAllAsync(
            filter: n => n.UserId == userId && !n.IsRead,
            tracked: true,
            cancellationToken: cancellationToken);

        if (!notifications.Any()) return false;

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}