using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Consumers;

public class GetUserResumePhotoConsumer(JobDbContext _jobDbContext) : IConsumer<GetUserResumePhotoRequest>
{
    public async Task Consume(ConsumeContext<GetUserResumePhotoRequest> context)
    {
        var response = await _jobDbContext.Resumes
            .Where(x => x.UserId == context.Message.UserId)
            .Select(x => new GetUserResumePhotoResponse
            {
                ResumeId = x.Id,
                ImageUrl = x.UserPhoto
            }).FirstOrDefaultAsync();
        
        await context.RespondAsync(new GetUserResumePhotoResponse
        {
            ResumeId = response.ResumeId,
            ImageUrl = response.ImageUrl
        });
    }
}
