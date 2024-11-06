using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using SharedLibrary.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }
    }
}
