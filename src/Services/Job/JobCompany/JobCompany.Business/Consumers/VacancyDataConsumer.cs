using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class VacancyDataConsumer : IConsumer<GetUserSavedVacanciesRequest>
    {
        private readonly JobCompanyDbContext _context;
        readonly IConfiguration _configuration;
        private readonly string? _authServiceBaseUrl;

        public VacancyDataConsumer(JobCompanyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
        }

        public async Task Consume(ConsumeContext<GetUserSavedVacanciesRequest> context)
        {
            var vacancyIds = context.Message.VacancyIds;

            var vacancies = await _context
                .Vacancies.Where(v => vacancyIds.Contains(v.Id))
                .AsNoTracking()
                .Skip(Math.Max(0, (context.Message.Skip - 1) * context.Message.Take))
                .Take(context.Message.Take)
                .ToListAsync();

            if (vacancies == null)
            {
                await context.RespondAsync(
                    new GetUserSavedVacanciesResponse { Vacancies = new List<VacancyResponse>() }
                );
                return;
            }

            var response = new GetUserSavedVacanciesResponse
            {
                Vacancies = vacancies
                    .Select(v => new VacancyResponse
                    {
                        Id = v.Id,
                        Title = v.Title,
                        CompanyName = v.CompanyName,
                        CompanyLocation = v.Location,
                        CreatedDate = v.StartDate,
                        CompanyPhoto = $"{_authServiceBaseUrl}/{v.CompanyLogo}",
                        MainSalary = v.MainSalary,
                        MaxSalary = v.MaxSalary,
                        ViewCount = v.ViewCount,
                        IsVip = v.IsVip,
                        WorkType = v.WorkType,
                        IsSaved = true,
                    })
                    .ToList(),
            };
            await context.RespondAsync(response);
        }
    }
}
