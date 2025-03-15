using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.NotificationDtos;

namespace JobCompany.Business.Services.NotificationServices
{
    public interface INotificationService
    {
        Task<DataListDto<NotificationDto>> GetUserNotificationsAsync(bool? IsSeen , int skip,int take);
        Task MarkNotificationAsReadAsync(Guid id);
        Task MarkAllNotificationAsReadAsync();
    }
}