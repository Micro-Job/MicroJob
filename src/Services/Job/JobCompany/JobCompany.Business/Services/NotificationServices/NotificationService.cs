using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.NotificationDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.NotificationServices
{
    public class NotificationService(JobCompanyDbContext _context, ICurrentUser _currentUser, IConfiguration _configuration,IRequestClient<GetUsersDataRequest> _usersDataClient) : INotificationService
    {
        public async Task<DataListDto<NotificationDto>> GetUserNotificationsAsync(bool? IsSeen, int skip, int take)
        {
            var query = _context.Notifications.Where(n => n.Receiver.UserId == _currentUser.UserGuid).AsNoTracking();

            if (IsSeen != null)
                query = query.Where(n => n.IsSeen == IsSeen).OrderByDescending(n => n.CreatedDate);

            var usersDataResponse = await _usersDataClient.GetResponse<GetUsersDataResponse>(new GetUsersDataRequest
            {
                UserIds = query.Where(n => n.SenderId.HasValue).Select(n => n.SenderId.Value).Distinct().ToList()
            });

            var usersDictionary = usersDataResponse.Message.Users.ToDictionary(u => u.UserId);

            var notifications = await query
                .Select(n => new
                {
                    Notification = n,
                    User = n.SenderId.HasValue ? usersDictionary.GetValueOrDefault(n.SenderId.Value) : null
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            var notificationDtos = notifications
                .Select(n => new NotificationDto
                {
                    Id = n.Notification.Id,
                    ReceiverId = n.Notification.ReceiverId,
                    SenderId = n.Notification.SenderId,
                    InformationId = n.Notification.InformationId,
                    InformationName = n.Notification.InformationName,
                    NotificationType = n.Notification.NotificationType,
                    CreatedDate = n.Notification.CreatedDate,
                    SenderImage = $"{_currentUser.BaseUrl}/user/{n.User?.ProfileImage}",
                    SenderName = n.User != null ? $"{n.User.FirstName} {n.User.LastName}" : null,
                    IsSeen = n.Notification.IsSeen,
                })
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
                                            ?? throw new NotFoundException<Notification>();

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
