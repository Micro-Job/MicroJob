using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;

namespace Job.Business.Consumers
{
    public class GetResumeDataConsumer(JobDbContext _jobDbContext) : IConsumer<GetResumeDataRequest>
    {
        public async Task Consume(ConsumeContext<GetResumeDataRequest> context)
        {
            var userIds = context.Message.UserIds;

            var resumes = await _jobDbContext.Resumes.Where(r => userIds.Contains(r.UserId))
                .Select(r => new GetResumeDataResponse
                {
                    UserId = r.UserId,
                    Position = r.Position != null ? r.Position.Name : null,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    ProfileImage = r.UserPhoto
                })
                .ToListAsync();

            await context.RespondAsync(new GetResumesDataResponse { Users = resumes });
        }
    }
}
