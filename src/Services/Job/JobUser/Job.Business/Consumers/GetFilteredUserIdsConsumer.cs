using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Consumers;

public class GetFilteredUserIdsConsumer(JobDbContext _jobDbContext) : IConsumer<GetFilteredUserIdsRequest>
{
    /// <summary> Request-dəki parametrlərə uyğun olaraq, istifadəçi ID-lərini filtr edir. (Müraciətlərin göründüyü view-da filtrləmə üçün) </summary>
    public async Task Consume(ConsumeContext<GetFilteredUserIdsRequest> context)
    {
        var query = _jobDbContext.Resumes.Where(r => context.Message.UserIds.Contains(r.UserId));

        if (context.Message.SkillIds != null && context.Message.SkillIds.Count != 0)
        {
            query = query.Include(x=> x.ResumeSkills).Where(r => context.Message.SkillIds.All(skillId => r.ResumeSkills.Any(rs => rs.SkillId == skillId))); // Skill ID-lərinə görə filtrləmə
        }

        var matchingUserIds = await query.Select(r => r.UserId).ToListAsync();

        await context.RespondAsync(new GetFilteredUserIdsResponse { UserIds = matchingUserIds });
    }
}

