using StajYerim.Application.DTOs.Notification;
using StajYerim.Application.Interfaces;
using StajYerim.Core.Common;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;

namespace StajYerim.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> CreateNotificationAsync(int userId, string title, string message, string? type = null, int? relatedEntityId = null, CancellationToken cancellationToken = default)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedEntityId = relatedEntityId,
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            };

            await _notificationRepository.AddNotificationAsync(notification, cancellationToken);
            await _unitOfWork.CompleteAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<List<NotificationDto>>> GetUserNotificationsAsync(int userId, CancellationToken cancellationToken = default)
        {
            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId, cancellationToken);

            var dtoList = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                Type = n.Type,
                RelatedEntityId = n.RelatedEntityId,
                CreatedAt = n.CreatedDate
            }).ToList();

            return Result<List<NotificationDto>>.Success(dtoList);
        }

        public async Task<Result<int>> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default)
        {
            var count = await _notificationRepository.GetUnreadCountAsync(userId, cancellationToken);
            
            return Result<int>.Success(count);
        }

        public async Task<Result<bool>> MarkAsReadAsync(int notificationId, int userId, CancellationToken cancellationToken = default)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId, userId, cancellationToken);

            if (notification == null)
                return Result<bool>.Failure("Bildirim bulunamadı.");

            notification.IsRead = true;
            notification.UpdatedDate = DateTime.UtcNow;
            
            _notificationRepository.UpdateNotification(notification);
            await _unitOfWork.CompleteAsync();

            return Result<bool>.Success(true);
        }
    }
}
