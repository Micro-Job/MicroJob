using JobCompany.Business.Exceptions.Common;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Dtos.VacancyDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Consumers
{
    public class GetVacancyInfoConsumer(JobCompanyDbContext context, IConfiguration configuration) : IConsumer<GetVacancyInfoRequest>
    {
        private readonly JobCompanyDbContext _context = context;
        readonly IConfiguration _configuration = configuration;
        private readonly string? _authServiceBaseUrl = configuration["AuthService:BaseUrl"];

        public async Task Consume(ConsumeContext<GetVacancyInfoRequest> context)
        {
            var vacancyId = context.Message.Id;
            var vacancy =
                await _context
                    .Vacancies.Where(x => x.Id == vacancyId)
                    .Select(v => new
                    {
                        Vacancy = v,
                        v.Category,
                        v.Company,
                        VacancyNumbers = v.VacancyNumbers.Select(n => n.Number).ToList(),
                    })
                    .FirstOrDefaultAsync() ?? throw new NotFoundException<Vacancy>();

            vacancy.Vacancy.ViewCount++;
            await _context.SaveChangesAsync();

            var response = new GetVacancyInfoResponse
            {
                Id = vacancy.Vacancy.Id,
                MainSalary = vacancy.Vacancy.MainSalary,
                MaxSalary = vacancy.Vacancy.MaxSalary,
                Email = vacancy.Vacancy.Email,
                Military = vacancy.Vacancy.Military,
                Driver = vacancy.Vacancy.Driver,
                Citizenship = vacancy.Vacancy.Citizenship,
                IsActive = vacancy.Vacancy.VacancyStatus,
                Gender = vacancy.Vacancy.Gender,
                Family = vacancy.Vacancy.Family,
                CompanyName = vacancy.Vacancy.CompanyName,
                Title = vacancy.Vacancy.Title,
                CompanyLogo = $"{_authServiceBaseUrl}/{vacancy.Company.CompanyLogo}",
                Requirement = vacancy.Vacancy.Requirement,
                Description = vacancy.Vacancy.Description,
                VacancyNumbers = vacancy
                    .VacancyNumbers.Select(n => new NumberDto { VacancyNumber = n })
                    .ToList(),
                StartDate = vacancy.Vacancy.StartDate,
                EndDate = vacancy.Vacancy.EndDate,
                //CategoryName = vacancy.Category.CategoryName,
                WorkType = vacancy.Vacancy.WorkType,
                WorkStyle = vacancy.Vacancy.WorkStyle,
                Location = vacancy.Vacancy.Location,
                ViewCount = vacancy.Vacancy.ViewCount,
                CompanyId = vacancy.Company.Id,
                HasExam = vacancy.Vacancy.ExamId != null,
            };
            await context.RespondAsync(response);
        }
    }
}