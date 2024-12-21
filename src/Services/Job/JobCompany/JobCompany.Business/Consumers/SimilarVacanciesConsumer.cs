using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;

namespace JobCompany.Business.Consumers
{
    public class SimilarVacanciesConsumer(JobCompanyDbContext context) : IConsumer<SimilarVacanciesRequest>
    {
        private readonly JobCompanyDbContext _context = context;

        public async Task Consume(ConsumeContext<SimilarVacanciesRequest> context)
        {
            var vacancyId = context.Message.VacancyId;
            var guidVacId = Guid.Parse(vacancyId);

            var vacancies = await _context.Vacancies
                .Include(v => v.Company)
                .Where(v => v.Id == guidVacId)
                .Select(v => new
                {
                    v.CategoryId,
                    SimilarVacancies = _context.Vacancies
                        .Where(s => s.CategoryId == v.CategoryId && s.Id != guidVacId)
                        .Take(6)
                        .Select(sim => new SimilarVacancyResponse
                        {
                            Id = sim.Id,
                            Title = sim.Title,
                            CompanyName = sim.CompanyName,
                            CompanyLocation = sim.Location,
                            CreatedDate = sim.StartDate,
                            CompanyPhoto = sim.CompanyLogo,
                            MainSalary = sim.MainSalary,
                            MaxSalary = sim.MaxSalary,
                            ViewCount = sim.ViewCount,
                            IsVip = sim.IsVip,
                            WorkType = sim.WorkType,
                            CategoryId = sim.Category.Id
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (vacancies == null || vacancies.CategoryId == Guid.Empty)
            {
                await context.RespondAsync(new SimilarVacanciesResponse
                {
                    Vacancies = []
                });
                return;
            }

            var response = new SimilarVacanciesResponse
            {
                Vacancies = vacancies.SimilarVacancies
            };

            await context.RespondAsync(response);
        }
    }
}