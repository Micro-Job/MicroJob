using Job.Business.Dtos.NotificationDtos;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Services.Notification
{
    public class NotificationService(JobDbContext context) : INotificationService
    {
        private readonly JobDbContext _context = context;

        public async Task CreateNotificationAsync(NotificationDto notificationDto)
        {
            var notification = new Core.Entities.Notification
            {
                Id = Guid.NewGuid(),
                ReceiverId = notificationDto.ReceiverId,
                SenderId = notificationDto.SenderId,
                CreatedDate = DateTime.Now,
                Content = notificationDto.Content,
                IsSeen = false
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationByUserDto>> GetUserNotificationsAsync(Guid userId)
        {

            var notifications = await _context.Notifications
                .Where(n => n.ReceiverId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
            return null;
        }

        public async Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if (notification != null)
            {
                notification.IsSeen = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}