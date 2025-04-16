using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.NotificationDtos;

namespace JobCompany.Business.Services.NotificationServices
{
    public interface INotificationService
    {
        Task<DataListDto<NotificationDto>> GetUserNotificationsAsync(bool? IsSeen , int skip,int take);
        Task MarkNotificationAsReadAsync(string id);
        Task MarkAllNotificationAsReadAsync();

        Task CreateNotificationAsync(CreateNotificationDto dto);
        Task CreateBulkNotificationAsync(CreateBulkNotificationDto dto);
    }
}