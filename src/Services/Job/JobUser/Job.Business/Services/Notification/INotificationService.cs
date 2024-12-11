using Job.Business.Dtos.NotificationDtos;

namespace Job.Business.Services.Notification
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(NotificationDto notificationDto);
        Task<List<NotificationByUserDto>> GetUserNotificationsAsync(Guid userId);
        Task MarkNotificationAsReadAsync(Guid id);
    }
}