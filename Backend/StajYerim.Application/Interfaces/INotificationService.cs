using StajYerim.Application.DTOs.Notification;
using StajYerim.Core.Common;

namespace StajYerim.Application.Interfaces
{
    public interface INotificationService
    {
        Task<Result<bool>> CreateNotificationAsync(int userId, string title, string message, string? type = null, int? relatedEntityId = null, CancellationToken cancellationToken = default);
        Task<Result<List<NotificationDto>>> GetUserNotificationsAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result<bool>> MarkAsReadAsync(int notificationId, int userId, CancellationToken cancellationToken = default);
        Task<Result<int>> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default);
    }
}
