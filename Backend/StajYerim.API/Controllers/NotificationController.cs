using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajYerim.Application.Interfaces;
using System.Security.Claims;

namespace StajYerim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int GetUserId()
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(claimValue))
            {
                throw new UnauthorizedAccessException("Token geçersiz.");
            }
            return int.Parse(claimValue);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _notificationService.GetUserNotificationsAsync(userId, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _notificationService.GetUnreadCountAsync(userId, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _notificationService.MarkAsReadAsync(id, userId, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}
