using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class GetAllCompaniesConsumer(ICurrentUser _currentUser, JobCompanyDbContext _context) : IConsumer<GetAllCompaniesRequest>
    {
        public async Task Consume(ConsumeContext<GetAllCompaniesRequest> context)
        {
            var searchTerm = context.Message.SearchTerm?.ToLower() ?? string.Empty;

            var companiesQuery = _context.Companies.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                companiesQuery = companiesQuery.Where(x =>
                    x.CompanyName.ToLower().Contains(searchTerm)
                );
            }
            var totalCount = await companiesQuery.CountAsync();

            var companies = await companiesQuery
                .Skip(Math.Max(0, (context.Message.Skip - 1) * context.Message.Take))
                .Take(context.Message.Take)
                .Select(x => new CompanyDto
                {
                    CompanyId = x.Id,
                    CompanyUserId = x.UserId,
                    CompanyName = x.CompanyName,
                    CompanyImage = $"{_currentUser.BaseUrl}/{x.CompanyLogo}",
                    CompanyVacancyCount = x.Vacancies.Count,
                })
                .ToListAsync();
            await context.RespondAsync(new GetAllCompaniesResponse { Companies = companies, TotalCount = totalCount });
        }
    }
}