using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using Shared.Events;

namespace Job.Business.Consumers
{
    public class VacancyUpdatedConsumer(JobDbContext _context) : IConsumer<VacancyUpdatedEvent>
    {
        public async Task Consume(ConsumeContext<VacancyUpdatedEvent> context)
        {
            var notifications = context
                .Message.UserIds.Select(userId => new Notification
                {
                    ReceiverId = userId,
                    SenderId = context.Message.SenderId,
                    NotificationType = SharedLibrary.Enums.NotificationType.VacancyUpdate,
                    InformationId = context.Message.InformationId,
                    IsSeen = false,
                })
                .ToList();

            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }
    }
}
