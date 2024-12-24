using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using SharedLibrary.Events;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Consumers
{
    public class VacancyCreatedConsumer : IConsumer<VacancyCreatedEvent>
    {
        private readonly JobDbContext _context;

        public VacancyCreatedConsumer(JobDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<VacancyCreatedEvent> context)
        {
            var eventMessage = context.Message;

            var resumes = await _context.Resumes
                .Include(r => r.ResumeSkills)  
                .Where(r => r.ResumeSkills.Any(rs => eventMessage.SkillIds.Contains(rs.SkillId))) 
                .ToListAsync();

            var notifications = new List<Notification>();

            foreach (var resume in resumes)
            {
                var newNotification = new Notification
                {
                    ReceiverId = resume.UserId,  
                    SenderId = eventMessage.SenderId, 
                    Content = eventMessage.Content, 
                    InformationId = eventMessage.InformationId,
                    IsSeen = false 
                };

                notifications.Add(newNotification);  
            }

            if (notifications.Any())
            {
                await _context.Notifications.AddRangeAsync(notifications);
                await _context.SaveChangesAsync();
            }
        }
    }
}
