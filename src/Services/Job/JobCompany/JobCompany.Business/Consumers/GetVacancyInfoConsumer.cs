using JobCompany.Business.Exceptions.Common;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Dtos.VacancyDtos;
using SharedLibrary.Dtos.CategoryDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Consumers
{
    public class GetVacancyInfoConsumer : IConsumer<GetVacancyInfoRequest>
    {
        private readonly JobCompanyDbContext _context;
        readonly IConfiguration _configuration;
        private readonly string? _authServiceBaseUrl;

        public GetVacancyInfoConsumer(JobCompanyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
        }

        public async Task Consume(ConsumeContext<GetVacancyInfoRequest> context)
        {
            var vacancyId = context.Message.Id;
            var vacancy = await _context.Vacancies
                .Include(v => v.Category)
                .Include(v => v.Company)
                .Include(v => v.VacancyNumbers)
                .FirstOrDefaultAsync(x => x.Id == vacancyId)
                ?? throw new NotFoundException<Vacancy>();

            vacancy.ViewCount++;
            await _context.SaveChangesAsync();

            var response = new GetVacancyInfoResponse
            {
                Id = vacancy.Id,
                MainSalary = vacancy.MainSalary,
                MaxSalary = vacancy.MaxSalary,
                Email = vacancy.Email,
                Military = vacancy.Military,
                Driver = vacancy.Driver,
                Citizenship = vacancy.Citizenship,
                IsActive = vacancy.IsActive,
                Gender = vacancy.Gender,
                Family = vacancy.Family,
                CompanyName = vacancy.CompanyName,
                Title = vacancy.Title,
                CompanyLogo = $"{_authServiceBaseUrl}/{vacancy.Company.CompanyLogo}",
                Requirement = vacancy.Requirement,
                Description = vacancy.Description,
                VacancyNumbers = vacancy.VacancyNumbers.Select(n => new NumberDto { VacancyNumber = n.Number }).ToList(),
                StartDate = vacancy.StartDate,
                EndDate = vacancy.EndDate,
                CategoryName = vacancy.Category.CategoryName,
                WorkType = vacancy.WorkType,
                Location = vacancy.Location,
                ViewCount = vacancy.ViewCount,
                CompanyId = vacancy.Company.Id,
            };
            await context.RespondAsync(response);
        }
    }
}