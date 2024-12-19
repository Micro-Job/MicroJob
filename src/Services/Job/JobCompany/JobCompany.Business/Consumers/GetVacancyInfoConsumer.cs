using JobCompany.Business.Exceptions.Common;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.CategoryDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Consumers
{
    public class GetVacancyInfoConsumer(JobCompanyDbContext context) : IConsumer<GetVacancyInfoRequest>
    {
        private readonly JobCompanyDbContext _context = context;
        public async Task Consume(ConsumeContext<GetVacancyInfoRequest> context)
        {
            var vacancyId = context.Message.Id;
            var vacancy = await _context.Vacancies
                .Include(v => v.Category)
                .FirstOrDefaultAsync(x => x.Id == vacancyId)
                ?? throw new NotFoundException<Vacancy>();

            var response = new GetVacancyInfoResponse
            {
                Id = vacancy.Id,
                CompanyName = vacancy.Company.CompanyName,
                Title = vacancy.Title,
                CompanyLogo = vacancy.Company.CompanyLogo,
                MainSalary = vacancy.MainSalary,
                Requirement = vacancy.Requirement,
                Description = vacancy.Description,
                StartDate = vacancy.StartDate,
                EndDate = vacancy.EndDate,
                CategoryName = vacancy.Category.CategoryName,
                WorkType = vacancy.WorkType,
                Location = vacancy.Location,
                ViewCount = vacancy.ViewCount,
                Family = vacancy.Family,
                Gender = vacancy.Gender,
                Military = vacancy.Military,
                Driver = vacancy.Driver,
                Citizenship = vacancy.Citizenship,
                IsActive = vacancy.IsActive,
                // VacancyNumbers = vacancy.VacancyNumbers.Select(x => x.Vacancy.VacancyNumbers).ToList()
            };
            await context.RespondAsync(response);
        }
    }
}