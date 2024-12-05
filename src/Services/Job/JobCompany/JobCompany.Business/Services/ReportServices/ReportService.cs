using JobCompany.Business.Dtos.ReportDtos;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;

namespace JobCompany.Business.Services.ReportServices
{
    public class ReportService(JobCompanyDbContext context, IRequestClient<GetUsersDataResponse> client) : IReportService
    {
        private readonly JobCompanyDbContext _context = context;
        readonly IRequestClient<GetUsersDataResponse> _client = client;

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

        /// <summary>
        /// admin/dashboard son muracietler
        /// </summary>
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

            var userDataResponse = await GetUserDataResponseAsync(userIds);

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

        public async Task<GetUsersDataResponse> GetUserDataResponseAsync(List<Guid> userIds)
        {
            var request = new GetUsersDataRequest
            {
                UserIds = userIds
            };
            var response = await _client.GetResponse<GetUsersDataResponse>(request);
            var userDataResponse = new GetUsersDataResponse
            {
                Users = response.Message.Users
            };
            return userDataResponse;
        }
    }
}