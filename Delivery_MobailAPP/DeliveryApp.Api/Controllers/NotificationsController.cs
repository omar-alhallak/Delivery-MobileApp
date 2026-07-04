using DeliveryApp.Application.Features.Notifications;
using DeliveryApp.Application.Features.Notifications.Common;
using DeliveryApp.Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/notifications")]
    public sealed class NotificationsController : ControllerBase // Controller خاص بقراءة إشعارات المستخدم وتعليمها كمقروءة
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("my")]
        public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetMine(CancellationToken ct) // جلب إشعارات المستخدم الحالي
            => Ok(await _notificationService.GetMineAsync(GetCurrentUserId(), ct));

        [HttpPatch("{id:guid}/read")]
        public Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct) // تعليم إشعار واحد كمقروء
            => RunChange(() => _notificationService.MarkAsReadAsync(GetCurrentUserId(), id, ct));

        [HttpPatch("read-all")]
        public Task<IActionResult> MarkAllAsRead(CancellationToken ct) // تعليم كل إشعارات المستخدم الحالي كمقروءة
            => RunChange(() => _notificationService.MarkAllAsReadAsync(GetCurrentUserId(), ct));

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new UnauthorizedAccessException("Invalid user id.");

            return parsedUserId;
        }

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
