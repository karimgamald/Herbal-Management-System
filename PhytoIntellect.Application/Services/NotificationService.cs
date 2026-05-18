using PhytoIntellect.Application.Contracts.Notifications;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
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

    // ================= Admin Bulk Notification Service =================

    public async Task SendBulkNotificationAsync(AdminNotificationRequest request, CancellationToken cancellationToken = default)
    {
        IQueryable<int> targetUserIds;

        // الفرز وجلب المعرفات بناءً على الفئة المستهدفة للـ Admin
        if (request.TargetRole == AppRoles.Herbalist)
        {
            targetUserIds = unitOfWork.HerbalistRepository.GetQueryable(tracked: false).Select(h => h.UserId);
        }
        else if (request.TargetRole == AppRoles.Patient)
        {
            targetUserIds = unitOfWork.PatientRepository.GetQueryable(tracked: false).Select(p => p.UserId);
        }
        else // في حالة اختيار "All" لجميع مستخدمي النظام الأساسيين من الفئتين
        {
            var herbalistUsers = unitOfWork.HerbalistRepository.GetQueryable(tracked: false).Select(h => h.UserId);
            var patientUsers = unitOfWork.PatientRepository.GetQueryable(tracked: false).Select(p => p.UserId);
            targetUserIds = herbalistUsers.Union(patientUsers);
        }

        var userIdsList = targetUserIds.ToList();
        if (!userIdsList.Any()) return;

        // تجهيز قائمة الإشعارات لعمل Bulk Insert سريع وموفر لموارد الخادم
        var notifications = userIdsList.Select(userId => new Notification
        {
            UserId = userId,
            Title = request.Title,
            Message = request.Message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        foreach (var notification in notifications)
        {
            await unitOfWork.NotificationRepository.CreateAsync(notification, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}