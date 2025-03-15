using JobCompany.Business.Services.NotificationServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    public class NotificationController(INotificationService _notificationService) : ControllerBase
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