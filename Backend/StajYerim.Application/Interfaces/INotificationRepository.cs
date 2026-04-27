using StajYerim.Core.Entities;

namespace StajYerim.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
        Task<List<Notification>> GetUserNotificationsAsync(int userId, CancellationToken cancellationToken = default);
        Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default);
        Task<Notification?> GetNotificationByIdAsync(int notificationId, int userId, CancellationToken cancellationToken = default);
        void UpdateNotification(Notification notification);
    }
}
