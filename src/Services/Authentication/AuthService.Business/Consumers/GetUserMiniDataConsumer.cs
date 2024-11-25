using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Business.Services.CurrentUser;
using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;

namespace AuthService.Business.Consumers
{
    public class GetUserMiniDataConsumer(AppDbContext _context) : IConsumer<GetAllCompaniesDataRequest>
    {
        public async Task Consume(ConsumeContext<GetAllCompaniesDataRequest> context)
        {
            var user = await _context.Users
                .Where(x => x.Id == context.Message.UserId)
                .Select(x => new 
                {
                    x.Email,
                    x.MainPhoneNumber
                })
                .FirstOrDefaultAsync();

            if (user is null)
            {
                await context.RespondAsync<GetAllCompaniesDataResponse>(null);
                return;
            }

            await context.RespondAsync(new GetAllCompaniesDataResponse
            {
                Email = user.Email,
                PhoneNumber = user.MainPhoneNumber
            });
        }
    }
}
