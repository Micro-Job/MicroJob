using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Services.Notification;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserNotifications()
        {
            var notifications = await _service.GetUserNotificationsAsync();
            return Ok(notifications);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MarkNotificationAsRead(Guid notificationId)
        {
            await _service.MarkNotificationAsReadAsync(notificationId);
            return Ok();
        }
    }
}