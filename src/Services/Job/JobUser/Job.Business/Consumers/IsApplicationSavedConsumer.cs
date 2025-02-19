using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Consumers;
public class IsApplicationSavedConsumer(JobDbContext context) : IConsumer<IsApplicationSavedRequest>
{
    private readonly JobDbContext _context = context;

    public async Task Consume(ConsumeContext<IsApplicationSavedRequest> context)
    {
        var request = context.Message;

        var isSaved = await _context.SavedVacancies
            .AnyAsync(s => s.UserId.ToString() == request.UserId && s.VacancyId.ToString() == request.VacancyId);

        await context.RespondAsync(new IsApplicationSavedResponse { IsSaved = isSaved });
    }
}