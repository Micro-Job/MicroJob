using Job.Business.Dtos.NotificationDtos;

namespace Job.Business.Services.Notification
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(NotificationDto notificationDto);
        Task<List<NotificationListDto>> GetUserNotificationsAsync(int skip = 1, int take = 6);
        Task MarkNotificationAsReadAsync(Guid id);
    }
}