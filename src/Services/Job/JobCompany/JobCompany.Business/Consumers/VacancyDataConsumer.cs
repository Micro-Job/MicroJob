using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Business.Services.CurrentUser;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class VacancyDataConsumer : IConsumer<GetUserSavedVacanciesRequest>
    {
        private readonly JobCompanyDbContext _context;

        public VacancyDataConsumer(JobCompanyDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<GetUserSavedVacanciesRequest> context)
        {
            var vacancyIds = context.Message.VacancyIds;

            var vacancies = await _context.Vacancies
                .Where(v => vacancyIds.Contains(v.Id))
                .ToListAsync();

            if (vacancies == null || !vacancies.Any())
            {
                await context.RespondAsync(new GetUserSavedVacanciesResponse
                {
                    Vacancies = new List<VacancyResponse>()
                });
                return;
            }

            var response = new GetUserSavedVacanciesResponse
            {
                Vacancies = vacancies.Select(v => new VacancyResponse
                {
                    Title = v.Title,
                    CompanyName = v.CompanyName,
                    CompanyLocation = v.Location,
                    CreatedDate = v.StartDate,
                    CompanyPhoto = v.CompanyLogo,
                    MainSalary = v.MainSalary,
                    MaxSalary = v.MaxSalary,
                    ViewCount = v.ViewCount ?? 0,
                    IsVip = v.IsVip,
                    WorkType = v.WorkType ?? SharedLibrary.Enums.WorkType.FullTime,
                }).ToList()
            };

            await context.RespondAsync(response);
        }
    }
}