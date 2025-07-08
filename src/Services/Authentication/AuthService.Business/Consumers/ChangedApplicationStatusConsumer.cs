using AuthService.Business.HelperServices.Email;
using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Consumers
{
    public class ChangedApplicationStatusConsumer(AppDbContext _context, EmailService _emailService) : IConsumer<ChangedApplicationStatusEvent>
    {
        public async Task Consume(ConsumeContext<ChangedApplicationStatusEvent> context)
        {
            var message = context.Message;

            var user = await _context.Users.Where(x => x.Id == message.UserId).Select(x=> new 
            { 
                x.Email,
                FullName = x.FirstName + ' ' + x.LastName
            }).FirstOrDefaultAsync();

            if(user == null)
            {
                await context.ConsumeCompleted;
                return;
            }

            _emailService.SendApplicationMessage(user.Email, user.FullName, message.Message, message.ApplicationId.ToString());
        }
    }
}
