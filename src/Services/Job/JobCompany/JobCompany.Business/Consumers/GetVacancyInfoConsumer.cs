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
                CompanyLogo = vacancy.CompanyLogo,
                Requirement = vacancy.Requirement,
                Description = vacancy.Description,
                StartDate = vacancy.StartDate,
                EndDate = vacancy.EndDate,
                CategoryName = vacancy.Category.CategoryName,
                WorkType = vacancy.WorkType,
                Location = vacancy.Location,
                ViewCount = vacancy.ViewCount,
            };
            await context.RespondAsync(response);
        }
    }
}