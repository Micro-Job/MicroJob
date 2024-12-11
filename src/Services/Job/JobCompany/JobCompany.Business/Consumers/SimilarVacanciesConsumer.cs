using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;

namespace JobCompany.Business.Consumers
{
    public class SimilarVacanciesConsumer : IConsumer<SimilarVacanciesRequest>
    {
        private readonly JobCompanyDbContext _context;

        public SimilarVacanciesConsumer(JobCompanyDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<SimilarVacanciesRequest> context)
        {
            var vacancyId = context.Message.VacancyId;
            var guidVacId = Guid.Parse(vacancyId);

            var vacancies = await _context.Vacancies
                .Where(v => v.Id == guidVacId ||
                            (v.CategoryId == _context.Vacancies
                                .Where(vac => vac.Id == guidVacId)
                                .Select(vac => vac.CategoryId)
                                .FirstOrDefault() && v.Id != guidVacId))
                .Take(6)
                .ToListAsync();

            var response = new SimilarVacanciesResponse
            {
                Vacancies = vacancies.Select(v => new SimilarVacancyResponse
                {
                    Title = v.Title,
                    CompanyName = v.Company.CompanyName,
                    CompanyLocation = v.Company.CompanyLocation,
                    CreatedDate = v.StartDate,
                    CompanyPhoto = v.Company.CompanyLogo,
                    MainSalary = v.MainSalary,
                    MaxSalary = v.MaxSalary,
                    ViewCount = v.ViewCount,
                    IsVip = v.IsVip,
                    IsSaved = false,
                    WorkType = v.WorkType
                }).ToList()
            };

            await context.RespondAsync(response);
        }
    }
}