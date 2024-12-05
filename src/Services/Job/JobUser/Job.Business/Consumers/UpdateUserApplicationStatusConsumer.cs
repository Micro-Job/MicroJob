using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using Shared.Events;

namespace Job.Business.Consumers
{
    public class UpdateUserApplicationStatusConsumer(JobDbContext _context) : IConsumer<UpdateUserApplicationStatusEvent>
    {
        public async Task Consume(ConsumeContext<UpdateUserApplicationStatusEvent> context)
        {
            var newNotification = new Notification
            {
                ReceiverId = context.Message.UserId,
                Content = context.Message.Content,
                IsSeen = false
            };

            await _context.Notifications.AddAsync(newNotification);
            await _context.SaveChangesAsync();
        }
    }
}