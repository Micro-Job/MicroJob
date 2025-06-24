using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using SharedLibrary.Enums;
using SharedLibrary.Events;

namespace Job.Business.Consumers;

public class UserRegisteredConsumer(JobDbContext _context) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var message = context.Message;

        var newUser = new User
        {
            Id = message.UserId,
            JobStatus = JobStatus.ActivelySeekingJob,
            FirstName = message.FirstName,
            LastName = message.LastName,
            Email = message.Email,
            MainPhoneNumber = message.MainPhoneNumber,
            Image = null
        };

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
    }
}
