using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.VacancyDtos;
using Shared.Requests;
using Shared.Responses;

namespace JobCompany.Business.Consumers
{
    public class GetAllVacanciesByCompanyIdConsumer(JobCompanyDbContext _context) : IConsumer<GetAllVacanciesByCompanyIdDataRequest>
    {
        public async Task Consume(ConsumeContext<GetAllVacanciesByCompanyIdDataRequest> context)
        {
            var vacancies = await _context.Vacancies
            .Where(x => x.CompanyId == context.Message.CompanyId)
            .Include(x => x.Company)
            .ToListAsync();

            if (vacancies is null)
            {
                await context.RespondAsync(new GetAllVacanciesByCompanyIdDataResponse
                {
                    Vacancies = []
                });
                return;
            }

            var response = new GetAllVacanciesByCompanyIdDataResponse
            {
                Vacancies = vacancies.Select(v => new AllVacanyDto
                {
                    CompanyName = v.Company.CompanyName,
                    CompanyLogo = v.Company.CompanyLogo,
                    Location = v.Company.CompanyLocation,
                    Title = v.Title,
                    WorkType = v.WorkType,
                    IsActive = v.IsActive,
                    IsVip = v.IsVip,
                    ViewCount = v.ViewCount,
                    MainSalary = v.MainSalary,
                    CategoryId = v.CategoryId,
                    StartDate = v.StartDate,
                }).ToList()
            };

            await context.RespondAsync(response);
        }
    }
}