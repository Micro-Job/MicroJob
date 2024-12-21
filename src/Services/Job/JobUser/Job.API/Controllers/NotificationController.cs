using Job.Business.Services.Notification;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.EmployeeUser)]
    public class NotificationController(INotificationService service) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserNotifications()
        {
            var notifications = await service.GetUserNotificationsAsync();
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