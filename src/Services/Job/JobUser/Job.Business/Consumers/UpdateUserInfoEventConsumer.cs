using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;

namespace Job.Business.Consumers;

public class UpdateUserInfoEventConsumer(JobDbContext _jobDbContext) : IConsumer<UpdateUserInfoEvent>
{
    public async Task Consume(ConsumeContext<UpdateUserInfoEvent> context)
    {
        var user = await _jobDbContext.Users.FirstOrDefaultAsync(x=> x.Id == context.Message.UserId) ?? throw new NotFoundException();

        user.FirstName = context.Message.FirstName;
        user.LastName = context.Message.LastName;

        await _jobDbContext.SaveChangesAsync();
    }
}
