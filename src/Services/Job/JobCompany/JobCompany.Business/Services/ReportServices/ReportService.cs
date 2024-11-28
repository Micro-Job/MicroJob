using JobCompany.Business.Dtos.ReportDtos;
using JobCompany.Core.Entites;
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
            int activeVacancies = await _context.Vacancies.CountAsync(v => v.IsActive);
            int totalApplications = await _context.Applications.CountAsync();
            IQueryable<Guid> acceptedStatusIdQuery = _context.Statuses
                 .Where(s => s.StatusName == "Accepted" && s.IsDefault)
                 .Select(s => s.Id)
                 .AsQueryable();

            IQueryable<Application> acceptedApplicationsQuery = _context.Applications
                .Where(a => acceptedStatusIdQuery.Contains(a.StatusId))
                .AsQueryable();

            int acceptedApplications = await acceptedApplicationsQuery.CountAsync();

            SummaryDto summary = new()
            {
                ActiveVacancies = activeVacancies,
                TotalApplications = totalApplications,
                AcceptedApplications = acceptedApplications
            };
            return summary;
        }
    }
}