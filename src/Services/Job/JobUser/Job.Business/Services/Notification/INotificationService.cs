using Job.Business.Dtos.NotificationDtos;

namespace Job.Business.Services.Notification
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(NotificationDto notificationDto);
        Task<List<Core.Entities.Notification>> GetUserNotificationsAsync();
        Task MarkNotificationAsReadAsync(Guid id);
    }
}