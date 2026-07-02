using DeliveryApp.Application.Features.Notifications;
using DeliveryApp.Application.Features.Notifications.Common;
using DeliveryApp.Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public sealed class NotificationsController : ControllerBase // Controller خاص بقراءة إشعارات المستخدم وتعليمها كمقروءة
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("users/{userId:guid}")]
        public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetByUser(Guid userId, CancellationToken ct) // جلب إشعارات مستخدم
            => Ok(await _notificationService.GetByUserAsync(userId, ct));

        [HttpPatch("{id:guid}/read")]
        public Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct) // تعليم إشعار واحد كمقروء
            => RunChange(() => _notificationService.MarkAsReadAsync(id, ct));

        [HttpPatch("users/{userId:guid}/read-all")]
        public Task<IActionResult> MarkAllAsRead(Guid userId, CancellationToken ct) // تعليم كل إشعارات المستخدم كمقروءة
            => RunChange(() => _notificationService.MarkAllAsReadAsync(userId, ct));

        private static async Task<IActionResult> RunChange(Func<Task<bool>> action) // توحيد ردود تعليم الإشعارات كمقروءة
        {
            try
            {
                var changed = await action();
                return changed ? new NoContentResult() : new NotFoundResult();
            }
            catch (DomainException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message });
            }
        }
    }
}
