using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;

namespace Job.Business.Consumers;

public class UpdateUserInfoEventConsumer(JobDbContext _jobDbContext) : IConsumer<UpdateUserInfoEvent>
{
    public async Task Consume(ConsumeContext<UpdateUserInfoEvent> context)
    {
        var user = await _jobDbContext.Users.FindAsync(context.Message.UserId) ?? throw new NotFoundException<User>();

        user.FirstName = context.Message.FirstName;
        user.LastName = context.Message.LastName;

        await _jobDbContext.SaveChangesAsync();
    }
}
