using Microsoft.EntityFrameworkCore;
using StajYerim.Application.Interfaces;
using StajYerim.Core.Entities;
using StajYerim.Infrastructure.Data;

namespace StajYerim.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly StajYerimDbContext _context;

        public NotificationRepository(StajYerimDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _context.Notifications.AddAsync(notification, cancellationToken);
        }

        public async Task<Notification?> GetNotificationByIdAsync(int notificationId, int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId && !n.IsDeleted, cancellationToken);
        }

        public async Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted, cancellationToken);
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public void UpdateNotification(Notification notification)
        {
            _context.Notifications.Update(notification);
        }
    }
}
