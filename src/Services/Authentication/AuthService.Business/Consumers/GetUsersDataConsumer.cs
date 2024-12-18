using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;

namespace AuthService.Business.Consumers
{
    public class GetUsersDataConsumer : IConsumer<GetUsersDataRequest>
    {
        private readonly AppDbContext _context;

        public GetUsersDataConsumer(AppDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<GetUsersDataRequest> context)
        {
            var userIds = context.Message.UserIds;

            var users = await _context.Users
                .Where(x => userIds.Contains(x.Id))
                .Select(x => new UserResponse
                {
                    UserId = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ProfileImage = x.Image,
                    Email = x.Email,
                    PhoneNumber = x.MainPhoneNumber
                })
                .ToListAsync();

            await context.RespondAsync(new GetUsersDataResponse
            {
                Users = users
            });
        }
    }
}