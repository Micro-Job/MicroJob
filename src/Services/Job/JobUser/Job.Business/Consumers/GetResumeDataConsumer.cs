using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;

namespace Job.Business.Consumers
{
    public class GetResumeDataConsumer(JobDbContext context) : IConsumer<GetResumeDataRequest>
    {
        private readonly JobDbContext _context = context;

        public async Task Consume(ConsumeContext<GetResumeDataRequest> context)
        {
            //var resume = await _context.Resumes
            //    .Include(r => r.User)
            //    .Where(r => context.Message.UserIds.Contains(r.UserId))
            //    .Select(r => new
            //    {
            //        r.UserId,
            //        r.Position
            //    })
            //    .FirstOrDefaultAsync();

            //if (resume is null)
            //{
            //    await context.RespondAsync(new GetResumeDataResponse { Position = null});
            //    return;
            //}

            //await context.RespondAsync(new GetResumeDataResponse
            //{
            //    Position = resume.Position
            //});

            var userIds = context.Message.UserIds;

            var resumes = await _context.Resumes
                .Where(r => userIds.Contains(r.UserId))
                .Select(r => new GetResumeDataResponse
                {
                    UserId = r.UserId,
                    Position = r.Position
                })
                .ToListAsync();

            await context.RespondAsync(new GetResumesDataResponse
            {
                Users = resumes
            });
        }
    }
}