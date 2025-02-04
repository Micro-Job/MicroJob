using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;

namespace JobCompany.Business.Consumers
{
    public class SimilarVacanciesConsumer(JobCompanyDbContext _dbContext) : IConsumer<SimilarVacanciesRequest>
    {
        public async Task Consume(ConsumeContext<SimilarVacanciesRequest> context)
        {
            var guidVacId = Guid.Parse(context.Message.VacancyId);

            var vacancies = await _dbContext
                .Vacancies.Include(v => v.Company)
                .Where(v => v.Id == guidVacId)
                .Select(v => new
                {
                    v.CategoryId,
                    SimilarVacancies = _dbContext
                        .Vacancies.Where(s => s.CategoryId == v.CategoryId && s.Id != guidVacId)
                        .Skip(context.Message.Skip)
                        .Take(context.Message.Take)
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
                            CategoryId = sim.Category.Id,
                        })
                        .ToList(),
                    TotalCount = _dbContext.Vacancies.Where(s => s.CategoryId == v.CategoryId && s.Id != guidVacId).Count()
                })
                .FirstOrDefaultAsync();

            if (vacancies == null || vacancies.CategoryId == Guid.Empty)
            {
                await context.RespondAsync(new SimilarVacanciesResponse { Vacancies = [], TotalCount = 0 });
                return;
            }

            var response = new SimilarVacanciesResponse { Vacancies = vacancies.SimilarVacancies, TotalCount = vacancies.TotalCount };

            await context.RespondAsync(response);
        }
    }
}
