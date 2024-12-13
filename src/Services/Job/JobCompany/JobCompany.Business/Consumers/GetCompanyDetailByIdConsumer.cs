using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Exceptions;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class GetCompanyDetailByIdConsumer(JobCompanyDbContext jobCompanyDbContext, IRequestClient<GetUserEmailRequest> requestClient) : IConsumer<GetCompanyDetailByIdRequest>
{
    private readonly JobCompanyDbContext _jobCompanyDbContext = jobCompanyDbContext;
    private readonly IRequestClient<GetUserEmailRequest> _requestClient = requestClient;
    public async Task Consume(ConsumeContext<GetCompanyDetailByIdRequest> context)
    {
        var company = await _jobCompanyDbContext.Companies.Include(x => x.CompanyNumbers).FirstOrDefaultAsync(x => x.Id == context.Message.CompanyId)
            ?? throw new NotFoundException<Company>();

        GetUserEmailRequest userEmailRequest = new() { UserId = company.UserId };

        var userEmailResponse = await _requestClient.GetResponse<GetUserEmailResponse>(userEmailRequest)
            ?? throw new Exception("Failed to retrieve user email");

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