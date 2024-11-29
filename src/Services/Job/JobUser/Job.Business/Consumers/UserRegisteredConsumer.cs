using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using SharedLibrary.Events;

namespace Job.Business.Consumers
{
    public class UserRegisteredConsumer(JobDbContext _context) : IConsumer<UserRegisteredEvent>
    {
        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var newUser = new User
            {
                Id = context.Message.UserId
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
        }
    }
}