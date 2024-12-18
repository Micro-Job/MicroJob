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

            await _context.Database.ExecuteSqlRawAsync(
                   "UPDATE Vacancies SET ViewCount = ISNULL(ViewCount, 0) + 1 WHERE Id = {0}", vacancyId);

            var vacancy = await _context.Vacancies
                .Include(v => v.Category)
                .FirstOrDefaultAsync(x => x.Id == vacancyId)
                ?? throw new NotFoundException<Vacancy>();

            var response = new GetVacancyInfoResponse
            {
                Requirement = vacancy.Requirement,
                Description = vacancy.Description,
                StartDate = vacancy.StartDate,
                EndDate = vacancy.EndDate,
                Category = vacancy.Category != null
                    ? new CategoryDto
                    {
                        Id = vacancy.Category.Id,
                        Name = vacancy.Category.CategoryName
                    }
                    : null,
                WorkType = vacancy.WorkType,
                Location = vacancy.Location,
                Family = vacancy.Family,
                Gender = vacancy.Gender,
                ViewCount = vacancy.ViewCount
            };
            await context.RespondAsync(response);
        }
    }
}