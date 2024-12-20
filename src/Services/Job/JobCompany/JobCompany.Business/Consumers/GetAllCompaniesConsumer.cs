using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class GetAllCompaniesConsumer : IConsumer<GetAllCompaniesRequest>
    {
        private readonly JobCompanyDbContext _context;
        readonly IConfiguration _configuration;
        private readonly string? _authServiceBaseUrl;

        public GetAllCompaniesConsumer(IConfiguration configuration, JobCompanyDbContext context)
        {
            _configuration = configuration;
            _context = context;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
        }


        public async Task Consume(ConsumeContext<GetAllCompaniesRequest> context)
        {
            var companies = await _context.Companies.Select(x => new CompanyDto
            {
                CompanyId = x.Id,
                CompanyName = x.CompanyName,
                CompanyImage = $"{_authServiceBaseUrl}/{x.CompanyLogo}",
                CompanyVacancyCount = x.Vacancies.Count
            }).ToListAsync();

            await context.RespondAsync(new GetAllCompaniesResponse
            {
                Companies = companies
            });
        }
    }
}