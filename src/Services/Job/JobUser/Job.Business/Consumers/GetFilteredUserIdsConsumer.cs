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

        if (context.Message.Gender != null)  
        {
            query = query.Where(r => r.Gender == context.Message.Gender); // Genderə görə filtrləmə
        }

        if (context.Message.SkillIds != null && context.Message.SkillIds.Count != 0)
        {
            query = query.Where(r => context.Message.SkillIds.All(skillId => r.ResumeSkills.Any(rs => rs.SkillId == skillId))); // Skill ID-lərinə görə filtrləmə

            query = query.Include(r => r.ResumeSkills);
        }

        var matchingUserIds = await query.Select(r => r.UserId).ToListAsync();

        await context.RespondAsync(new GetFilteredUserIdsResponse { UserIds = matchingUserIds });
    }
}

