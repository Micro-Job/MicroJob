using JobCompany.Business.Dtos.NotificationDtos;

namespace JobCompany.Business.Services.NotificationServices
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync();
        Task MarkNotificationAsReadAsync(Guid id);
    }
}