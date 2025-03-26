using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Consumers;

public class ResumeLookupConsumer(JobDbContext _dbContext) : IConsumer<ResumeLookupRequest>
{
    public async Task Consume(ConsumeContext<ResumeLookupRequest> context)
    {
        var userIds = context.Message.UserIds;

        var resumes = await _dbContext.Resumes
            .Where(r => userIds.Contains(r.UserId))
            .Select(r => new ResumeInfoDto
            {
                UserId = r.UserId,
                Gender = r.Gender,
                SkillIds = r.ResumeSkills.Select(rs => rs.SkillId).ToList()
            })
            .ToListAsync();

        await context.RespondAsync(new ResumeLookupResponse { Resumes = resumes });
    }
}
