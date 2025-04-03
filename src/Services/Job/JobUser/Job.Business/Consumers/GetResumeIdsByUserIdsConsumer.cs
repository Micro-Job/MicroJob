using Job.DAL.Contexts;
using MassTransit;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Consumers;

public class GetResumeIdsByUserIdsConsumer(JobDbContext _dbContext) : IConsumer<GetResumeIdsByUserIdsRequest>
{
    public async Task Consume(ConsumeContext<GetResumeIdsByUserIdsRequest> context)
    {
        var resumeIds = _dbContext.Resumes
            .Where(x => context.Message.UserIds.Contains(x.UserId))
            .Select(x => x.Id)
            .ToList();

        await context.RespondAsync(new GetResumeIdsByUserIdsResponse
        {
            ResumeIds = resumeIds
        });
    }
}
