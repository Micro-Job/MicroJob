using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.VacancyDtos;
using SharedLibrary.Events;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class UserVacancyConsumer(JobCompanyDbContext _companyDb) : IConsumer<UserRegisteredEvent>
    {
        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var userId = context.Message.UserId;
            var userVacancies = await _companyDb.Vacancies
                .Include(v => v.Company)
                .Where(v => v.Company.UserId == userId)
                .Select(v => new VacancyDto
                {
                    CompanyName = v.Company.CompanyName,
                    Title = v.Title,
                    CompanyLogo = v.Company.CompanyLogo,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    Location = v.Location,
                    ViewCount = v.ViewCount,
                    MainSalary = v.MainSalary,
                    WorkType = v.WorkType,
                    IsSaved = false,
                    IsVip = v.IsVip
                })
                .ToListAsync();

            await context.RespondAsync(new UserVacanciesResponse
            {
                Vacancies = userVacancies
            });
        }
    }
}