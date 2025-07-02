using JobCompany.Business.Services.NotificationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController(NotificationService _notificationService) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserNotificationsAsync(bool? IsSeen , int skip, int take)
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(IsSeen , skip , take);
            return Ok(notifications);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MarkNotificationAsReadAsync(Guid notificationId)
        {
            await _notificationService.MarkNotificationAsReadAsync(notificationId);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MarkAllNotificationAsRead()
        {
            await _notificationService.MarkAllNotificationAsReadAsync();
            return Ok();
        }

    }
}