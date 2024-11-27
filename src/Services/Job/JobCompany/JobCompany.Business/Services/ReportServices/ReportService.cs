using JobCompany.Business.Dtos.ReportDtos;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.ReportServices
{
    public class ReportService(JobCompanyDbContext context) : IReportService
    {
        private readonly JobCompanyDbContext _context = context;

        /// <summary>
        /// Get a summary of the vacancies and applications.
        /// </summary>
        /// <returns>
        /// A summary object containing:
        /// - Active vacancies count.
        /// - Total applications count.
        /// - Accepted applications count.
        /// </returns>
        public async Task<SummaryDto> GetSummaryAsync()
        {
            var activeVacancies = await _context.Vacancies.CountAsync(v => v.IsActive);
            var totalApplications = await _context.Applications.CountAsync();
            var acceptedStatusId = await _context.Statuses
            .Where(s => s.StatusName == "Accepted")
            .Select(s => s.Id)
            .FirstOrDefaultAsync();

            var acceptedApplications = acceptedStatusId == Guid.Empty
                ? 0
                : await _context.Applications.CountAsync(a => a.StatusId == acceptedStatusId);

            var summary = new SummaryDto()
            {
                ActiveVacancies = activeVacancies,
                TotalApplications = totalApplications,
                AcceptedApplications = acceptedApplications
            };
            return summary;
        }
    }
}