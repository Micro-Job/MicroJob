using System.Security.Claims;
using Job.Business.Dtos.NotificationDtos;
using Job.Business.Exceptions.UserExceptions;
using Job.Core.Entities;
using Job.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;

namespace Job.Business.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly JobDbContext _context;
        readonly IHttpContextAccessor _contextAccessor;
        private readonly Guid userGuid;
        public NotificationService(JobDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? throw new UserIsNotLoggedInException());
        }

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

        public async Task<List<Core.Entities.Notification>> GetUserNotificationsAsync()
        {
            var notifications = await _context.Notifications
                .Where(n => n.ReceiverId == userGuid)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
            return notifications;
        }

        public async Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId)
            ?? throw new NotFoundException<Core.Entities.Notification>();

            notification.IsSeen = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}