﻿using Job.Business.Dtos.NotificationDtos;

namespace Job.Business.Services.Notification
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(NotificationDto notificationDto);
        Task<PaginatedNotificationDto> GetUserNotificationsAsync(bool? IsSeen , int skip = 1, int take = 6);
        Task MarkNotificationAsReadAsync(Guid id);
        Task MarkAllNotificationAsReadAsync();
    }
}