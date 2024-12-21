using JobCompany.Business.Services.NotificationServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.CompanyUser)]
    public class NotificationController(INotificationService service) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserNotificationsAsync()
        {
            var notifications = await service.GetUserNotificationsAsync();
            return Ok(notifications);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MarkNotificationAsReadAsync(Guid notificationId)
        {
            await service.MarkNotificationAsReadAsync(notificationId);
            return Ok();
        }
    }
}