using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Consumers;

public class GetResumeIdsByUserIdsConsumer(JobDbContext _dbContext) : IConsumer<GetResumeIdsByUserIdsRequest>
{
    /// <summary> Verilmiş olan userId-lərə uyğun resumeId-ləri tapır. </summary>
    public async Task Consume(ConsumeContext<GetResumeIdsByUserIdsRequest> context)
    {
        var resumeIds = await _dbContext.Resumes
            .Where(r => context.Message.UserIds.Contains(r.UserId))
            .ToDictionaryAsync(r => r.UserId, r => r.Id);

        var response = new GetResumeIdsByUserIdsResponse { ResumeIds = resumeIds };

        await context.RespondAsync(response);
    }
}
