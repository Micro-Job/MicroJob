using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Events;

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

            //TODO : burada skill sayina ve resunedaki skill sayina nisbetde 50 faiz olarsa , o zaman getsin hemin adamlara notification

            var resumes = await _context
                    .Resumes.Include(r => r.ResumeSkills)
                    .Select(x=> new
                    {
                        UserId = x.UserId,
                        ResumeSkillIds = x.ResumeSkills.Select(x=> x.SkillId).ToList()
                    })
                    .AsSplitQuery()
                    .AsNoTracking()
                    .ToListAsync();

            var notifications = new List<Notification>();

            foreach (var resume in resumes)
            {
                var matchingSkillsCount = resume.ResumeSkillIds.Intersect(eventMessage.SkillIds).Count();

                if (matchingSkillsCount >= (eventMessage.SkillIds.Count / 2.0))
                {
                    var newNotification = new Notification
                    {
                        ReceiverId = resume.UserId,
                        SenderId = eventMessage.SenderId,
                        NotificationType = SharedLibrary.Enums.NotificationType.Vacancy,
                        InformationId = eventMessage.InformationId,
                        IsSeen = false,
                    };

                    notifications.Add(newNotification);
                }
            }

            if (notifications.Any())
            {
                await _context.Notifications.AddRangeAsync(notifications);
                await _context.SaveChangesAsync();
            }
        }
    }
}
