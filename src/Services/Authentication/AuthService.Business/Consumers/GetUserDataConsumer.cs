using AuthService.Business.Services.CurrentUser;
using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System.Threading.Tasks;

namespace AuthService.Business.Consumers
{
    public class GetUserDataConsumer(AppDbContext _context, ICurrentUser _currentUser) : IConsumer<GetUserDataRequest>
    {
        public async Task Consume(ConsumeContext<GetUserDataRequest> context)
        {
            var user = await _context.Users
                .Where(x => x.Id == context.Message.UserId)
                .Select(x => new 
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Email,
                    x.Image,
                    x.MainPhoneNumber
                })
                .FirstOrDefaultAsync();

            if (user is null)
            {
                await context.RespondAsync<GetUserDataResponse>(null);
                return;
            }

            await context.RespondAsync(new GetUserDataResponse
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfileImage = user.Image != null ? $"{_currentUser.BaseUrl}/{user.Image}" : null,
                MainPhoneNumber = user.MainPhoneNumber
            });
        }
    }
}
