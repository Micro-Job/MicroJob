using Job.Business.Dtos.NotificationDtos;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace Job.Business.Services.Notification
{
    public class NotificationService(JobDbContext _context , ICurrentUser _currentUser) : INotificationService
    {

        public async Task CreateNotificationAsync(NotificationDto notificationDto)
        {
            var notification = new Core.Entities.Notification
            {
                Id = Guid.NewGuid(),
                ReceiverId = notificationDto.ReceiverId,
                SenderId = notificationDto.SenderId,
                CreatedDate = DateTime.Now,
                InformationId = notificationDto.InformationId,
                InformationName = notificationDto.InformationName,
                IsSeen = false,
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedNotificationDto> GetUserNotificationsAsync(bool? IsSeen , int skip = 1,int take = 6)
        {
            var query = _context.Notifications.Where(n => n.ReceiverId == _currentUser.UserGuid).AsNoTracking().AsQueryable();

            if (IsSeen != null)
            {
                query = query.Where(n => n.IsSeen == IsSeen).OrderByDescending(n => n.CreatedDate);
            }
            else
            {
                query = query.OrderBy(n => n.IsSeen).ThenByDescending(n => n.CreatedDate);
            }


            var notifications = await query
            .Select(n => new NotificationListDto
            {
                Id = n.Id,
                ReceiverId = n.ReceiverId,
                SenderId = n.SenderId,
                InformationId = n.InformationId,
                CreatedDate = n.CreatedDate,
                InformationName= n.InformationName,
                NotificationType = n.NotificationType,
                IsSeen = n.IsSeen,
            })
            .Skip(Math.Max(0, (skip - 1) * take))
            .Take(take)
            .ToListAsync();

            return new PaginatedNotificationDto
            {
                Notifications = notifications,
                TotalCount = await query.CountAsync(),
            };
        }

        public async Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification =
                await _context.Notifications.FirstOrDefaultAsync(x=> x.Id == notificationId)
                ?? throw new NotFoundException<Core.Entities.Notification>(MessageHelper.GetMessage("NOT_FOUND"));

            notification.IsSeen = true;

            await _context.SaveChangesAsync();
        }

        public async Task MarkAllNotificationAsReadAsync()
        {
            await _context.Notifications
                .Where(x => x.ReceiverId == _currentUser.UserGuid && x.IsSeen == false)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.IsSeen, true));
        }
    }
}
