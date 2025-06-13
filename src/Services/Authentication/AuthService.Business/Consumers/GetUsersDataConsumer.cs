using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;

namespace AuthService.Business.Consumers;

public class GetUsersDataConsumer(AppDbContext _appDbContext) : IConsumer<GetUsersDataRequest>
{
    public async Task Consume(ConsumeContext<GetUsersDataRequest> context)
    {
        var userIds = context.Message.UserIds;

        var query = _appDbContext.Users.AsNoTracking();

        if (!string.IsNullOrEmpty(context.Message.FullName))
        {
            var fullName = context.Message.FullName.Trim().ToLower();
            query = query.Where(x => (x.FirstName + " " + x.LastName).ToLower().Contains(fullName));
        }

        var users = await query
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