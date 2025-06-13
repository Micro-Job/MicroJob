using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;

namespace AuthService.Business.Consumers
{
    public class GetUserMiniDataConsumer(AppDbContext _dbContext) : IConsumer<GetAllCompaniesDataRequest>
    {
        public async Task Consume(ConsumeContext<GetAllCompaniesDataRequest> context)
        {
            var response = await _dbContext.Users
                .AsNoTracking()
                .Where(x => x.Id == context.Message.UserId)
                .Select(x => new GetAllCompaniesDataResponse
                {
                    Email = x.Email,
                    PhoneNumber = x.MainPhoneNumber
                })
                .FirstOrDefaultAsync();

            if (response is null)
            {
                await context.RespondAsync<GetAllCompaniesDataResponse>(null);
                return;
            }

            await context.RespondAsync(response);
        }
    }
}