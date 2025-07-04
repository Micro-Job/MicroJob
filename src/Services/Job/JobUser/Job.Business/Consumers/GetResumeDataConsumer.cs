using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Consumers;

public class GetResumeDataConsumer(JobDbContext _jobDbContext) : IConsumer<GetResumeDataRequest>
{
    public async Task Consume(ConsumeContext<GetResumeDataRequest> context)
    {
        var resumeData = await _jobDbContext.Resumes
            .AsNoTracking()
            .Where(r => r.UserId == context.Message.UserId)
            .Select(r => new GetResumeDataResponse
            {
                ResumeId = r.Id,
                Position = r.Position != null ? r.Position.Name : null,
                FirstName = r.FirstName,
                LastName = r.LastName,
                ProfileImage = r.UserPhoto!,
                Email = r.ResumeEmail!,
                PhoneNumber = r.PhoneNumbers.Select(x=> x.PhoneNumber).FirstOrDefault()!,
                Gender = r.Gender
            }).FirstOrDefaultAsync();

        await context.RespondAsync(resumeData!);
    }
}
