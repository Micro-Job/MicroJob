using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Core.Entites;

namespace JobCompany.Business.Services.NotificationServices
{
    public interface INotificationService
    {
        Task<List<Notification>> GetUserNotificationsAsync();
        Task MarkNotificationAsReadAsync(Guid id);
    }
}