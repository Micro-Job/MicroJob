using AuthService.Business.Services.CurrentUser;
using AuthService.DAL.Contexts;
using Job.DAL.Contexts;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Consumers
{
    public class GetUserDataConsumer(AppDbContext _context,ICurrentUser _currentUser) : IConsumer<GetUserDataRequest>
    {
        public async Task Consume(ConsumeContext<GetUserDataRequest> context)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == context.Message.UserId);

            if(user is null)
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
