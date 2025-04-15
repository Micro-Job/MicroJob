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
            await context.RespondAsync(new GetCompaniesDataByUserIdsResponse { Companies = new Dictionary<Guid, CompanyNameAndImageDto>() });
            return;
        }

        var companies = await _jobCompanyDb.Companies
            .Where(u => userIds.Contains(u.UserId))
            .ToDictionaryAsync(
                u => u.UserId,
                u => new CompanyNameAndImageDto { CompanyName = u.CompanyName, CompanyLogo = u.CompanyLogo }
            );

        await context.RespondAsync(new GetCompaniesDataByUserIdsResponse { Companies = companies });
    }
}
