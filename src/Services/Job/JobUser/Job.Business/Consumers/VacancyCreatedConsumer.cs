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
        var requiredMatchCount = Math.Ceiling(context.Message.SkillIds.Count * 0.5);

        var matchedUserIds = await _dbContext.ResumeSkills
            .AsNoTracking()
            .Where(rs => context.Message.SkillIds.Contains(rs.SkillId))
            .GroupBy(rs => rs.Resume.UserId)
            .Where(g => g.Count() >= requiredMatchCount)
            .Select(g => g.Key)
            .ToListAsync();

        if (matchedUserIds.Count == 0) return;

        var notifications = matchedUserIds.Select(userId => new Notification
        {
            ReceiverId = userId,
            SenderId = context.Message.SenderId,
            NotificationType = NotificationType.Vacancy,
            InformationId = context.Message.InformationId,
            InformationName = context.Message.InformatioName,
            IsSeen = false
        }).ToList();

        if (notifications.Count > 0)
        {
            await _dbContext.Notifications.AddRangeAsync(notifications);
            await _dbContext.SaveChangesAsync();
        }
    }
}
