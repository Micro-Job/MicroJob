using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class GetAllCompaniesConsumer(JobCompanyDbContext _context) : IConsumer<GetAllCompaniesRequest>
    {
        public async Task Consume(ConsumeContext<GetAllCompaniesRequest> context)
        {
            var companies = await _context.Companies.Select(x => new CompanyDto
            {
                CompanyId = x.Id,
                CompanyName = x.CompanyName,
                CompanyImage = x.CompanyLogo,
                CompanyVacancyCount = x.Vacancies.Count
            }).ToListAsync();

            await context.RespondAsync(new GetAllCompaniesResponse
            {
                Companies = companies
            });
        }
    }
}