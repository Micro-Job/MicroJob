using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class CheckApplicationConsumer(JobCompanyDbContext _jobCompanyDb) : IConsumer<CheckApplicationRequest>
{
    /// <summary>
    /// Userin şirkətin hər hansı bir vakansiyasına müraciət edib etmədiyini yoxlayır.
    /// </summary>
    public async Task Consume(ConsumeContext<CheckApplicationRequest> context)
    {
        var hasApplied = await _jobCompanyDb.Applications.AnyAsync(app =>
            app.UserId == context.Message.UserId &&
            app.Vacancy.Company != null && app.Vacancy.Company.UserId == context.Message.CompanyUserId && !app.IsDeleted);

        await context.RespondAsync(new CheckApplicationResponse
        {
            HasApplied = hasApplied
        });
    }
}
