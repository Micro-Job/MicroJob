using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using SharedLibrary.Events;

namespace JobCompany.Business.Consumers;

public class VacancyAcceptConsumer(JobCompanyDbContext _context) : IConsumer<VacancyAcceptedEvent>
{
    public async Task Consume(ConsumeContext<VacancyAcceptedEvent> context)
    {
        var newNotification = new Notification
        {
            ReceiverId = context.Message.ReceiverId,
            SenderId = context.Message.SenderId,
            NotificationType = SharedLibrary.Enums.NotificationType.VacancyAccept,
            InformationId = context.Message.InformationId,
            InformationName = context.Message.InformationName,
            IsSeen = false,
        };
        await _context.Notifications.AddAsync(newNotification);
        await _context.SaveChangesAsync();
    }
}
