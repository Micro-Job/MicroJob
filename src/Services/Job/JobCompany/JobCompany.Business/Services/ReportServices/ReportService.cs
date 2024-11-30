using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Dtos.ReportDtos;
using JobCompany.Business.Services.ApplicationServices;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.ReportServices
{
    public class ReportService(JobCompanyDbContext _context,IApplicationService _appService) : IReportService
    {

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
        public async Task<List<RecentApplicationDto>> GetRecentApplicationsAsync()
        {
            var recentApplications = await _context.Applications
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(7)
                    .Select(a => new
                    {
                        a.UserId,
                        a.Vacancy.Title,
                        a.Status.StatusName,
                        a.Status.StatusColor
                    })
                    .ToListAsync();

            var userIds = recentApplications.Select(a => a.UserId).Distinct().ToList();

            var userDataResponse = await _appService.GetUserDataResponseAsync(userIds);

            var recentApplicationDtos = new List<RecentApplicationDto>();

            foreach (var application in recentApplications)
            {
                var userData = userDataResponse.Users.FirstOrDefault(u => u.UserId == application.UserId);

                if (userData != null)
                {
                    recentApplicationDtos.Add(new RecentApplicationDto
                    {
                        Fullname = $"{userData.FirstName} {userData.LastName}",
                        VacancyName = application.Title,
                        StatusName = application.StatusName,
                        StatusColor = application.StatusColor
                    });
                }
            }
            return recentApplicationDtos;
        }
    }
}