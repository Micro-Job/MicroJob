using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class GetCompaniesDataByUserIdsConsumer(JobCompanyDbContext _jobCompanyDb) : IConsumer<GetCompaniesDataByUserIdsRequest>
{
    /// <summary> User Id-lərinə görə şirkətlərin adını və loqosunu qaytarır. </summary>
    public async Task Consume(ConsumeContext<GetCompaniesDataByUserIdsRequest> context)
    {
        var userIds = context.Message.UserIds;

        if (userIds.Count == 0)
        {
            await context.RespondAsync(new GetCompaniesDataByUserIdsResponse { Companies = [] });
            return;
        }
        
        var query = _jobCompanyDb.Companies.Where(c => userIds.Contains(c.UserId));

        if (!string.IsNullOrEmpty(context.Message.CompanyName))
        {
            query = query.Where(c => c.CompanyName != null && c.CompanyName.ToLower().Contains(context.Message.CompanyName));
        }

        var companies = await query
            .ToDictionaryAsync(
                u => u.UserId,
                u => new CompanyNameAndImageDto { CompanyName = u.CompanyName, CompanyLogo = u.CompanyLogo }
            );

        await context.RespondAsync(new GetCompaniesDataByUserIdsResponse { Companies = companies });
    }
}
