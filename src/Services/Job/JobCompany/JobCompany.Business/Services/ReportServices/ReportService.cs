using System.Security.Claims;
using JobCompany.Business.Dtos.ReportDtos;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.ReportServices
{
    //TODO : Bu hissə yenidən yazılmalıdır
    public class ReportService(JobCompanyDbContext _context, ICurrentUser _currentUser)
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
        //public async Task<SummaryDto> GetSummaryAsync()
        //{
        //var acceptedStatusId = await _context
        //    .Statuses.Where(s => s.Name == "Accepted" && s.IsDefault)
        //    .Select(s => s.Id)
        //    .FirstOrDefaultAsync();

        //var result = await _context
        //    .Applications.GroupBy(a => new
        //    {
        //        a.Vacancy.IsActive,
        //        IsAccepted = a.StatusId == acceptedStatusId,
        //    })
        //    .Select(g => new
        //    {
        //        g.Key.IsActive,
        //        g.Key.IsAccepted,
        //        Count = g.Count(),
        //    })
        //    .ToListAsync();

        //var activeVacancies = result.Where(r => r.IsActive).Sum(r => r.Count);
        //var totalApplications = result.Sum(r => r.Count);
        //var acceptedApplications = result.Where(r => r.IsAccepted).Sum(r => r.Count);

        //var summary = new SummaryDto
        //{
        //    ActiveVacancies = activeVacancies,
        //    TotalApplications = totalApplications,
        //    AcceptedApplications = acceptedApplications,
        //};
        //return summary;
        //}

        /// <summary>
        /// admin/dashboard son muracietler
        /// </summary>
        //public async Task<List<RecentApplicationDto>> GetRecentApplicationsAsync()
        //{
        //    var recentApplications = await _context
        //        .Applications.OrderByDescending(a => a.CreatedDate)
        //        .Take(7)
        //        .Select(a => new
        //        {
        //            a.UserId,
        //            a.Vacancy.Title,
        //            StatusName = a.Status.GetTranslation(_currentUser.LanguageCode,GetTranslationPropertyName.Name),
        //            a.Status.StatusColor,
        //        })
        //        .ToListAsync();

        //    var userIds = recentApplications.Select(a => a.UserId).Distinct().ToList();

        //    var userDataResponse = await GetUserDataResponseAsync(userIds);

        //    var recentApplicationDtos = new List<RecentApplicationDto>();

        //    foreach (var application in recentApplications)
        //    {
        //        var userData = userDataResponse.Users.FirstOrDefault(u =>
        //            u.UserId == application.UserId
        //        );
        //    }
        //    return recentApplicationDtos;
        //}

        //public async Task<GetUsersDataResponse> GetUserDataResponseAsync(List<Guid> userIds)
        //{
        //    var request = new GetUsersDataRequest { UserIds = userIds };
        //    var response = await _client.GetResponse<GetUsersDataResponse>(request);
        //    var userDataResponse = new GetUsersDataResponse { Users = response.Message.Users };
        //    return userDataResponse;
        //}

        /// <summary> Applicationun statistikasi /// </summary>
        public async Task<ApplicationStatisticsDto> GetApplicationStatisticsAsync(byte periodTime)
        {
            var applications = await _context
                .Applications.Include(a => a.Vacancy)
                .Where(a => a.Vacancy.Company.UserId == _currentUser.UserGuid)
                .Select(a => new
                {
                    a.VacancyId,
                    a.Vacancy.Title,
                    a.CreatedDate,
                })
                .ToListAsync();

            IEnumerable<IGrouping<string, dynamic>> groupedApplications;
            switch (periodTime)
            {
                case 1:
                    groupedApplications = applications
                        .GroupBy(a => $"{a.CreatedDate.Year} - Week {GetWeekNumber(a.CreatedDate)}")
                        .OrderByDescending(g => g.Key);
                    break;

                case 2:
                    groupedApplications = applications
                        .GroupBy(a => $"{a.CreatedDate.ToString("MMMM yyyy")}")
                        .OrderByDescending(g => g.Key);
                    break;

                case 3:
                    groupedApplications = applications
                        .GroupBy(a => a.CreatedDate.ToString("yyyy"))
                        .OrderByDescending(g => g.Key);
                    break;

                default:
                    throw new ArgumentException("Invalid period time");
            }
            var currentPeriodApplications = groupedApplications.FirstOrDefault();
            var previousPeriodApplications = groupedApplications.Skip(1).FirstOrDefault();

            var currentPeriodCount = currentPeriodApplications?.Count() ?? 0;
            var previousPeriodCount = previousPeriodApplications?.Count() ?? 0;

            double percentageChange =
                previousPeriodCount > 0
                    ? (double)(currentPeriodCount - previousPeriodCount) / previousPeriodCount * 100
                    : 0;

            var mostCommonCount = groupedApplications.Any()
                ? groupedApplications.Max(g => g.Count())
                : 0;

            var groupedStatistics = groupedApplications
                .Select(g => new PeriodStatisticDto
                {
                    Period = g.Key,
                    Value = g.Count(),
                    IsHighlighted = g.Count() == mostCommonCount,
                })
                .ToList();

            var applicationDetails = applications
                .GroupBy(a => a.VacancyId)
                .Select(g => new ApplicationReportDetailDto 
                {
                    Position = g.FirstOrDefault()?.Title,
                    ApplicationsCount = g.Count(),
                })
                .ToList();

            var vacancyStatisticsDto = new ApplicationStatisticsDto
            {
                TotalApplications = applications.Count,
                PercentageChange = new PercentageChangeDto
                {
                    Value = percentageChange,
                    IsPositive = percentageChange > 0,
                },
                PeriodStatistics = groupedStatistics,
                Applications = applicationDetails,
            };

            return vacancyStatisticsDto;
        }

        private int GetWeekNumber(DateTime date)
        {
            var calendar = System.Globalization.CultureInfo.InvariantCulture.Calendar;
            return calendar.GetWeekOfYear(
                date,
                System.Globalization.CalendarWeekRule.FirstDay,
                DayOfWeek.Monday
            );
        }

        public async Task<SummaryDto> GetSummaryAsync()
        {
            throw new NotImplementedException();
        }
    }
}
