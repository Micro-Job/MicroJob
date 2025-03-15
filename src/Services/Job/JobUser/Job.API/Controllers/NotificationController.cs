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
        public async Task<IActionResult> GetUserNotifications(bool? IsSeen , int skip = 1,int take = 6)
        {
            var notifications = await service.GetUserNotificationsAsync(IsSeen , skip, take);
            return Ok(notifications);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MarkNotificationAsRead(Guid notificationId)
        {
            await service.MarkNotificationAsReadAsync(notificationId);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MarkAllNotificationAsRead()
        {
            await service.MarkAllNotificationAsReadAsync();
            return Ok();
        }
    }
}