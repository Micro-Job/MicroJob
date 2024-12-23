using Job.Business.Services.Notification;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.SimpleUser)]
    public class NotificationController(INotificationService service) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserNotifications([FromQuery] int skip = 1, [FromQuery] int take = 6)
        {
            var notifications = await service.GetUserNotificationsAsync(skip, take);
            return Ok(notifications);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MarkNotificationAsRead(Guid notificationId)
        {
            await service.MarkNotificationAsReadAsync(notificationId);
            return Ok();
        }
    }
}