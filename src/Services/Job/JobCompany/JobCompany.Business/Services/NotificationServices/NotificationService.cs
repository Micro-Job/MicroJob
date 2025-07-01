using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.NotificationDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.NotificationServices
{
    public class NotificationService(JobCompanyDbContext _context, ICurrentUser _currentUser)
    {
        public async Task<DataListDto<NotificationDto>> GetUserNotificationsAsync(bool? IsSeen, int skip, int take)
        {
            var query = _context.Notifications
                .Where(n => n.Receiver.UserId == _currentUser.UserGuid)
                .OrderByDescending(n => n.CreatedDate)
                .AsNoTracking();
                                                                                                    
            if (IsSeen != null)
                query = query.Where(n => n.IsSeen == IsSeen);

            var notificationDtos = query
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    ReceiverId = n.ReceiverId,
                    SenderId = n.SenderId,
                    InformationId = n.InformationId,
                    InformationName = n.InformationName,
                    NotificationType = n.NotificationType,
                    CreatedDate = n.CreatedDate,
                    SenderImage = $"{_currentUser.BaseUrl}/userFiles/{n.SenderImage}",
                    SenderName = n.SenderName,
                    IsSeen = n.IsSeen,
                })
                .Skip((skip - 1) * take)
                .Take(take)
                .ToList();

            return new DataListDto<NotificationDto>
            {
                Datas = notificationDtos,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task MarkNotificationAsReadAsync(string id)
        {
            var notificationGuid = Guid.Parse(id);

            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == notificationGuid && x.Receiver.UserId == _currentUser.UserGuid)
                                            ?? throw new NotFoundException();

            notification.IsSeen = true;

            await _context.SaveChangesAsync();
        }

        public async Task MarkAllNotificationAsReadAsync()
        {
            await _context.Notifications
                .Where(x => x.Receiver.UserId == _currentUser.UserGuid && x.IsSeen == false)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.IsSeen, true));
        }

        public async Task CreateNotificationAsync(CreateNotificationDto dto)
        {
            Notification notification = new Notification
            {
                InformationId = dto.InformationId,
                CreatedDate = DateTime.Now,
                InformationName = dto.InformationName,
                NotificationType = dto.NotificationType,
                IsSeen = false,
                ReceiverId = dto.ReceiverId,
                SenderId = dto.SenderId
            };

            await _context.Notifications.AddAsync(notification);
        }

        public async Task CreateBulkNotificationAsync(CreateBulkNotificationDto dto)
        {
            List<Notification> notifications = new List<Notification>();

            foreach (var receiverId in dto.ReceiverIds)
            {
                notifications.Add(new Notification
                {
                    InformationId = dto.InformationId,
                    CreatedDate = DateTime.Now,
                    InformationName = dto.InformationName,
                    NotificationType = dto.NotificationType,
                    IsSeen = false,
                    ReceiverId = receiverId,
                    SenderId = dto.SenderId
                });
            }

            await _context.Notifications.AddRangeAsync(notifications);
        }
    }
}
