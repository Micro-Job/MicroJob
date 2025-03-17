using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Shared.Events;

namespace JobCompany.Business.Consumers
{
    public class VacancyApplicationConsumer(JobCompanyDbContext _context)
        : IConsumer<VacancyApplicationEvent>
    {
        public async Task Consume(ConsumeContext<VacancyApplicationEvent> context)
        {
            var newNotification = new Notification
            {
                ReceiverId = context.Message.UserId,
                SenderId = context.Message.SenderId,
                //Content = context.Message.Content,
                InformationId = context.Message.InformationId,
                IsSeen = false,
            };
            await _context.Notifications.AddAsync(newNotification);
            await _context.SaveChangesAsync();
        }
    }
}