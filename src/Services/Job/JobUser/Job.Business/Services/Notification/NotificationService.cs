using Job.Business.Dtos.NotificationDtos;
using Job.Business.Exceptions.UserExceptions;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Responses;
using SharedLibrary.Exceptions;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System.Security.Claims;

namespace Job.Business.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly JobDbContext _context;
        readonly IHttpContextAccessor _contextAccessor;
        private readonly IRequestClient<GetAllCompaniesRequest> _getCompaniesClient;
        private readonly Guid userGuid;
        public NotificationService(JobDbContext context, IHttpContextAccessor contextAccessor, IRequestClient<GetAllCompaniesRequest> getCompaniesClient)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _getCompaniesClient = getCompaniesClient;
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

        public async Task<List<NotificationListDto>> GetUserNotificationsAsync(int skip = 1, int take = 6)
        {
            var companies = await GetAllCompaniesData();

            var companyDictionary = companies.Companies
                .GroupBy(x => x.CompanyUserId)
                .ToDictionary(g => g.Key, g => g.First());

            var notifications = await _context.Notifications
                .Where(n => n.ReceiverId == userGuid)
                .OrderByDescending(n => n.CreatedDate)
                .Skip(Math.Max(0, (skip - 1) * take))
                .ToListAsync();

            var notificationDtos = notifications.Select(n =>
            {
                companyDictionary.TryGetValue(n.SenderId, out var company);

                return new NotificationListDto
                {
                    ReceiverId = n.ReceiverId,
                    SenderId = n.SenderId,
                    CompanyName = company?.CompanyName,
                    CompanyLogo = company?.CompanyImage,
                    CreatedDate = n.CreatedDate,
                    Content = n.Content,
                    IsSeen = n.IsSeen
                };
            }).ToList();

            return notificationDtos;
        }

        public async Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId)
            ?? throw new NotFoundException<Core.Entities.Notification>();

            notification.IsSeen = true;

            await _context.SaveChangesAsync();
        }

        private async Task<GetAllCompaniesResponse> GetAllCompaniesData()
        {
            var request = new GetAllCompaniesRequest();

            var response = await _getCompaniesClient.GetResponse<GetAllCompaniesResponse>(request);

            return response.Message;
        }
    }
}