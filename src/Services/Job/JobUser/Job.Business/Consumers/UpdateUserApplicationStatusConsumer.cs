using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using Shared.Events;

namespace Job.Business.Consumers
{
    public class UpdateUserApplicationStatusConsumer(JobDbContext _context)
        : IConsumer<UpdateUserApplicationStatusEvent>
    {
        public async Task Consume(ConsumeContext<UpdateUserApplicationStatusEvent> context)
        {
            var newNotification = new Notification
            {
                ReceiverId = context.Message.UserId,
                SenderId = context.Message.SenderId,
                Content = context.Message.Content,
                InformationId = context.Message.InformationId,
                IsSeen = false,
            };

            await _context.Notifications.AddAsync(newNotification);
            await _context.SaveChangesAsync();
        }
    }
}
