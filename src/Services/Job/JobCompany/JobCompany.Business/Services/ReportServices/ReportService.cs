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

        /// <summary> Vakansiyanin statistikasi 
        /// Vakansiyalar isActive yoxsa hamisi ?
        /// </summary>
        /// 
        public async Task<ApplicationStatisticsDto> GetApplicationStatisticsAsync(string periodTime)
        {
            var applications = await _context.Applications
                                           .Include(a => a.Vacancy)
                                           .Select(a => new
                                           {
                                               a.VacancyId,
                                               a.Vacancy.Title,
                                               a.CreatedDate
                                           })
                                           .ToListAsync();

            IEnumerable<IGrouping<string, dynamic>> groupedApplications;

            if (periodTime == "1")
            {
                groupedApplications = applications
                    .GroupBy(a => $"{a.CreatedDate.Year}-{GetWeekNumber(a.CreatedDate)}")
                    .OrderByDescending(g => g.Key);
            }
            else if (periodTime == "2")
            {
                groupedApplications = applications
                    .GroupBy(a => a.CreatedDate.ToString("yyyy-MM"))
                    .OrderByDescending(g => g.Key);
            }
            else if (periodTime == "3")
            {
                groupedApplications = applications
                    .GroupBy(a => a.CreatedDate.ToString("yyyy"))
                    .OrderByDescending(g => g.Key);
            }
            else
            {
                throw new ArgumentException("Invalid period time");
            }

            var currentPeriodApplications = groupedApplications.FirstOrDefault();
            var previousPeriodApplications = groupedApplications.Skip(1).FirstOrDefault();

            var currentPeriodCount = currentPeriodApplications?.Count() ?? 0;
            var previousPeriodCount = previousPeriodApplications?.Count() ?? 0;

            double percentageChange = previousPeriodCount > 0
                ? (double)(currentPeriodCount - previousPeriodCount) / previousPeriodCount * 100
                : 0;

            var groupedStatistics = groupedApplications
                .Select(g => new PeriodStatisticDto
                {
                    Period = g.Key,
                    Value = g.Count(),
                    // IsHighlighted = g.Count() 
                    //en cox olanin countu
                })
                .ToList();

            var applicationDetails = applications
                .GroupBy(a => a.VacancyId)
                .Select(g => new ApplicationDetailDto
                {
                    Position = g.FirstOrDefault()?.Title,
                    ApplicationsCount = g.Count()
                })
                .ToList();

            var vacancyStatisticsDto = new ApplicationStatisticsDto
            {
                TotalApplications = applicationDetails.Count,
                PercentageChange = new PercentageChangeDto
                {
                    Value = percentageChange,
                    IsPositive = percentageChange > 0
                },
                PeriodStatistics = groupedStatistics,
                Applications = applicationDetails
            };

            return vacancyStatisticsDto;
        }

        private int GetWeekNumber(DateTime date)
        {
            var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            return calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }
}