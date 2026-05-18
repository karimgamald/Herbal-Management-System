using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(int userId, string title, string message, CancellationToken cancellationToken = default);
    Task<IEnumerable<object>> GetMyNotificationsAsync(int userId, bool? isRead = false, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(int notificationId, int userId, CancellationToken cancellationToken = default);

    Task<bool> DeleteNotificationAsync(int notificationId, int userId, CancellationToken cancellationToken = default);
    Task<bool> MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default);
}