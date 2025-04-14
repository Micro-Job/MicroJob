using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using SharedLibrary.Events;

namespace Job.Business.Consumers;

public class VacancyDeletedConsumer(JobDbContext _context) : IConsumer<VacancyDeletedEvent>
{
    public async Task Consume(ConsumeContext<VacancyDeletedEvent> context)
    {
        var notifications = context
            .Message.UserIds.Select(userId => new Notification
            {
                ReceiverId = userId,
                SenderId = context.Message.SenderId,
                NotificationType = SharedLibrary.Enums.NotificationType.VacancyDeleted,
                InformationId = context.Message.InformationId,
                InformationName = context.Message.InformationName,
                IsSeen = false,
            }).ToList();

        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();
    }
}
