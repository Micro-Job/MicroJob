using AuthService.Core.Entities;
using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;

namespace AuthService.Business.Consumers;

public class UpdateUserJobStatusEventConsumer(AppDbContext _dbContext) : IConsumer<UpdateUserJobStatusEvent>
{
    public async Task Consume(ConsumeContext<UpdateUserJobStatusEvent> context)
    {
        var updated = await _dbContext.Users
            .Where(u => u.Id == context.Message.UserId)
            .ExecuteUpdateAsync(b => b.SetProperty(u => u.JobStatus, _ => context.Message.JobStatus));

        if (updated == 0)
            throw new NotFoundException<User>();
    }
}
