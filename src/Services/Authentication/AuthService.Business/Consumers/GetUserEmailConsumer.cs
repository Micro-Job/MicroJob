using AuthService.Core.Entities;
using AuthService.DAL.Contexts;
using MassTransit;
using SharedLibrary.Exceptions;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace AuthService.Business.Consumers;

public class GetUserEmailConsumer(AppDbContext dbContext) : IConsumer<GetUserEmailRequest>
{
    public async Task Consume(ConsumeContext<GetUserEmailRequest> context)
    {
        var user = await dbContext.Users.FindAsync(context.Message.UserId)
            ?? throw new NotFoundException<User>();

        await context.RespondAsync(new GetUserEmailResponse
        {
            Email = user.Email,
            MainPhoneNumber = user.MainPhoneNumber
        });
    }
}