using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.Common;
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

        public async Task<DataListDto<NotificationDto>> GetUserNotificationsAsync(bool? IsSeen , int skip , int take)
        {
            var query = _context.Notifications.Where(n => n.Receiver.UserId == _currentUser.UserGuid).AsNoTracking().AsQueryable();

            if(IsSeen != null)
            {
                query = query.Where(n => n.IsSeen == IsSeen).OrderByDescending(n => n.CreatedDate);
            }
            else
            {
                query = query.OrderBy(n => n.IsSeen).ThenByDescending(n => n.CreatedDate);
            }

            var notifications = await query
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
            .Skip(Math.Max(0, (skip - 1) * take))
            .Take(take)
            .ToListAsync();

            return new DataListDto<NotificationDto>
            {
                Datas = notifications,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task MarkNotificationAsReadAsync(string id)
        {
            var notificationGuid = Guid.Parse(id);

            var notification = await _context.Notifications.FirstOrDefaultAsync(x =>x.Id == notificationGuid && x.Receiver.UserId == _currentUser.UserGuid) 
                                            ?? throw new NotFoundException<Notification>(MessageHelper.GetMessage("NOT_FOUND"));

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
