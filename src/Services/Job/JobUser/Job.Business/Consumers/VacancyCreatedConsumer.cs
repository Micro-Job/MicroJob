using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;
using SharedLibrary.Events;

namespace Job.Business.Consumers;

/// <summary>
/// Yeni vakansiya yaradıldıqda tələb olunan bacarıqların ən azı 50%-nə uyğun gələn istifadəçiləri tapır
/// və onlara vakansiyanın yaradılması ilə bağlı bildiriş göndərir.
/// </summary>
public class VacancyCreatedConsumer(JobDbContext _dbContext) : IConsumer<VacancyCreatedEvent>
{
    public async Task Consume(ConsumeContext<VacancyCreatedEvent> context)
    {
        VacancyCreatedEvent message = context.Message;

        double requiredMatchCount = Math.Ceiling(message.SkillIds!.Count * 0.5);

        List<Guid> matchedUserIds = await _dbContext.ResumeSkills
            .AsNoTracking()
            .Where(rs => message.SkillIds.Contains(rs.SkillId))
            .GroupBy(rs => rs.Resume.UserId)
            .Where(g => g.Count() >= requiredMatchCount)
            .Select(g => g.Key)
            .ToListAsync();

        if (!matchedUserIds.Any())
        {
            await context.ConsumeCompleted; 
            return;
        }
        else
        {
            var notifications = matchedUserIds.Select(userId => new Notification
            {
                ReceiverId = userId,
                SenderId = message.SenderId,
                NotificationType = NotificationType.Vacancy,
                InformationId = message.InformationId,
                InformationName = message.InformatioName,
                SenderName = message.SenderName,
                SenderImage = message.SenderImage,
                CreatedDate = DateTime.Now.AddHours(4),
                IsSeen = false
            }).ToList();

            await _dbContext.Notifications.AddRangeAsync(notifications);
            await _dbContext.SaveChangesAsync();
        }
    }
}
