using AuthService.Core.Entities;
using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;

namespace AuthService.Business.Consumers;

public class UpdateUserJobStatusEventConsumer(AppDbContext _context) : IConsumer<UpdateUserJobStatusEvent>
{
    public async Task Consume(ConsumeContext<UpdateUserJobStatusEvent> context)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == context.Message.UserId) ?? throw new NotFoundException<User>();
        user.JobStatus = context.Message.JobStatus;
        await _context.SaveChangesAsync();
    }
}
