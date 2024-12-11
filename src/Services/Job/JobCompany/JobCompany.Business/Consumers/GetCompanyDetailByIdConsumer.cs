using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Exceptions;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class GetCompanyDetailByIdConsumer(JobCompanyDbContext jobDbContext, IRequestClient<GetUserEmailRequest> userEmailClient) : IConsumer<GetCompanyDetailByIdRequest>
{
    public async Task Consume(ConsumeContext<GetCompanyDetailByIdRequest> context)
    {
        var company = await jobDbContext.Companies.Include(x => x.CompanyNumbers).FirstOrDefaultAsync(x => x.Id == context.Message.CompanyId)
            ?? throw new NotFoundException<Company>();

        GetUserEmailRequest userEmailRequest = new() { UserId = company.UserId };

        var userEmailResponse = await userEmailClient.GetResponse<GetUserEmailResponse>(userEmailRequest);

        await context.RespondAsync(new GetCompanyDetailByIdResponse
        {
            CompanyInformation = company.CompanyInformation,
            CompanyLocation = company.CompanyLocation,
            WebLink = company.WebLink,
            CompanyNumbers = company.CompanyNumbers?.Select(x => new CompanyNumberDto { Number = x.Number }).ToList(),
            Email = userEmailResponse.Message.Email
        });
    }
}
