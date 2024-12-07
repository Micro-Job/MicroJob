﻿using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Dtos.ReportDtos;
using JobCompany.Business.Services.ApplicationServices;
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
        public async Task<VacancyStatisticsDto> GetVacancyStatisticsAsync()
        {
            var vacancies = await _context.Vacancies.ToListAsync();
            var applications = await _context.Applications
                                              //   .Where(a => a.Vacancy.IsActive)
                                              .ToListAsync();

            var totalVacancies = vacancies.Count;
            // .(v => v.IsActive);
            var monthlyStatistics = new List<MonthlyStatisticDto>();
            var groupedApplications = applications
                .GroupBy(a => a.CreatedDate.ToString("yyyy-MM"))
                .OrderByDescending(g => g.Key);

            var currentMonthApplications = groupedApplications.FirstOrDefault();
            var previousMonthApplications = groupedApplications.Skip(1).FirstOrDefault();

            var previousMonthCount = previousMonthApplications?.Count() ?? 0;

            var currentMonthCount = currentMonthApplications?.Count() ?? 0;

            double percentageChange = 0;
            bool isPositive = false;

            if (previousMonthCount > 0)
            {
                percentageChange = (double)(currentMonthCount - previousMonthCount) / previousMonthCount * 100;
                isPositive = percentageChange > 0;
            }

            var percentageChangeDto = new PercentageChangeDto
            {
                Value = percentageChange,
                IsPositive = isPositive
            };

            monthlyStatistics = groupedApplications
               .Select(g => new MonthlyStatisticDto
               {
                   Month = g.Key,
                   Value = g.Count(),
                   IsHighlighted = g.Count() > 100
               }).ToList();
               
            var applicationDetails = vacancies
            .Select(v => new ApplicationDetailDto
            {
                Position = v.Title,
                ApplicationsCount = applications.Count(a => a.VacancyId == v.Id)
            }).ToList();

            var vacancyStatisticsDto = new VacancyStatisticsDto
            {
                TotalVacancies = totalVacancies,
                PercentageChange = percentageChangeDto,
                MonthlyStatistics = monthlyStatistics,
                Applications = applicationDetails
            };

            return vacancyStatisticsDto;
        }
    }
}