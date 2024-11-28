using JobCompany.Business.Dtos.ReportDtos;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.ReportServices
{
    public class ReportService(JobCompanyDbContext context) : IReportService
    {
        private readonly JobCompanyDbContext _context = context;

        /// <summary>
        /// admin/dashboard yuxaridaki 3-luk
        /// </summary>
        /// <returns>
        /// A summary object containing:
        /// - Active vacancies count.
        /// - Total applications count.
        /// - Accepted applications count.
        /// </returns>
        public async Task<SummaryDto> GetSummaryAsync()
        {
            var acceptedStatusId = await _context.Statuses
                .Where(s => s.StatusName == "Accepted" && s.IsDefault)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            var result = await _context.Applications
                .GroupBy(a => new { a.Vacancy.IsActive, IsAccepted = a.StatusId == acceptedStatusId })
                .Select(g => new
                {
                    g.Key.IsActive,
                    g.Key.IsAccepted,
                    Count = g.Count()
                })
                .ToListAsync();

            var activeVacancies = result.Where(r => r.IsActive).Sum(r => r.Count);
            var totalApplications = result.Sum(r => r.Count);
            var acceptedApplications = result.Where(r => r.IsAccepted).Sum(r => r.Count);

            var summary = new SummaryDto
            {
                ActiveVacancies = activeVacancies,
                TotalApplications = totalApplications,
                AcceptedApplications = acceptedApplications
            };
            return summary;
        }
    }
}