using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class CheckCompanyConsumer(JobCompanyDbContext companyDb) : IConsumer<CheckCompanyRequest>
{
    public async Task Consume(ConsumeContext<CheckCompanyRequest> context)
    {
        var company = await companyDb.Companies.FirstOrDefaultAsync(c =>
            c.Id == context.Message.CompanyId
        );

        var response = new CheckCompanyResponse { IsExist = company != null };

        await context.RespondAsync(response);
    }
}
