using System.Security.Claims;
using Job.Business.Dtos.NotificationDtos;
using Job.Business.Exceptions.UserExceptions;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Responses;
using SharedLibrary.Exceptions;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.Notification
{
    public class NotificationService(JobDbContext _context , IRequestClient<GetAllCompaniesRequest> _getCompaniesClient , ICurrentUser _currentUser) : INotificationService
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
                Content = notificationDto.Content,
                IsSeen = false,
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedNotificationDto> GetUserNotificationsAsync(
            int skip = 1,
            int take = 6
        )
        {
            //var companies = await GetAllCompaniesData();

            //var companyDictionary = companies
            //    .Companies.GroupBy(x => x.CompanyUserId)
            //    .ToDictionary(g => g.Key, g => g.First());

            var query = _context
                .Notifications.Where(n => n.ReceiverId == _currentUser.UserGuid);

            var notificationDtos = await query
                .Select(n => new NotificationListDto
                {
                    //companyDictionary.TryGetValue(n.SenderId, out var company);

                        Id = n.Id,
                        ReceiverId = n.ReceiverId,
                        SenderId = n.SenderId,
                        // CompanyName = company?.CompanyName,
                        // CompanyLogo = company?.CompanyImage,
                        InformationId = n.InformationId,
                        CreatedDate = n.CreatedDate,
                        Content = n.Content,
                        IsSeen = n.IsSeen,
                })
                .OrderByDescending(n => n.CreatedDate)
                .Skip(Math.Max(0, (skip - 1) * take))
                .ToListAsync();

            return new PaginatedNotificationDto
            {
                Notifications = notificationDtos,
                TotalCount = await query.CountAsync(),
            };
        }

        public async Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification =
                await _context.Notifications.FirstOrDefaultAsync(x=> x.Id == notificationId)
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
