using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JobCompany.Business.Exceptions.UserExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;

namespace JobCompany.Business.Services.NotificationServices
{
    public class NotificationService : INotificationService
    {
        private readonly JobCompanyDbContext _context;

        readonly IHttpContextAccessor _contextAccessor;
        private readonly Guid userGuid;

        public NotificationService(JobCompanyDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? throw new UserIsNotLoggedInException());
        }
        public async Task<List<Notification>> GetUserNotificationsAsync()
        {
            var notifications = await _context.Notifications
                .Where(n => n.ReceiverId == userGuid)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
            return notifications;
        }

        public async Task MarkNotificationAsReadAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id)
            ?? throw new NotFoundException<Notification>();

            notification.IsSeen = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}