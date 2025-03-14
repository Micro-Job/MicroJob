using JobCompany.Business.Dtos.NotificationDtos;

namespace JobCompany.Business.Services.NotificationServices
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(int skip,int take);
        Task MarkNotificationAsReadAsync(Guid id);
        Task MarkAllNotificationAsReadAsync();
    }
}