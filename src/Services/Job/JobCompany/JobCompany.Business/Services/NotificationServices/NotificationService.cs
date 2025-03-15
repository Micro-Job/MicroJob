using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.NotificationDtos;
using JobCompany.Business.Exceptions.UserExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.NotificationServices
{
    public class NotificationService(JobCompanyDbContext _context , ICurrentUser _currentUser) : INotificationService
    {

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int skip , int take)
        {
            var notifications = await _context
                .Notifications.Where(n => n.Receiver.UserId == _currentUser.UserGuid)
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    ReceiverId = n.ReceiverId,
                    SenderId = n.SenderId,
                    InformationId = n.InformationId,
                    CreatedDate = n.CreatedDate,
                    //Content = n.Content,
                    IsSeen = n.IsSeen,
                })
                .Skip(Math.Max(0,(skip - 1)*take))
                .Take(take)
                .ToListAsync();

            return notifications;
        }

        public async Task MarkNotificationAsReadAsync(Guid id)
        {
            var companyId = await _context
                .Companies.Where(x => x.UserId == _currentUser.UserGuid)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (companyId == Guid.Empty)
                throw new NotFoundException<Company>(MessageHelper.GetMessage("NOT_FOUND"));

            var notification =
                await _context.Notifications.FirstOrDefaultAsync(x =>
                    x.Id == id && x.ReceiverId == companyId
                ) ?? throw new NotFoundException<Notification>(MessageHelper.GetMessage("NOT_FOUND"));

            notification.IsSeen = true;

            await _context.SaveChangesAsync();
        }

        public async Task MarkAllNotificationAsReadAsync()
        {
            await _context.Notifications
                .Where(x => x.Receiver.UserId == _currentUser.UserGuid && x.IsSeen == false)
                .ExecuteUpdateAsync(x=> x.SetProperty(y=> y.IsSeen , true));
        }

    }
}
