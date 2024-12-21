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
        public async Task<List<NotificationDto>> GetUserNotificationsAsync()
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.UserId == userGuid)
                ?? throw new NotFoundException<Company>();

            var notifications = await _context.Notifications
                .Where(n => n.ReceiverId == company.Id)
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    ReceiverId = n.ReceiverId,
                    SenderId = n.SenderId,
                    CreatedDate = n.CreatedDate,
                    Content = n.Content,
                    IsSeen = n.IsSeen
                })
                .ToListAsync();

            return notifications;
        }

        public async Task MarkNotificationAsReadAsync(Guid id)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.UserId == userGuid)
                ?? throw new NotFoundException<Company>();

            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == company.Id)
                ?? throw new NotFoundException<Notification>();

            notification.IsSeen = true;

            await _context.SaveChangesAsync();
        }
    }
}