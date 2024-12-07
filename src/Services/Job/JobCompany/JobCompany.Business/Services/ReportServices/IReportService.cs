using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Dtos.ReportDtos;

namespace JobCompany.Business.Services.ReportServices
{
    public interface IReportService
    {
        Task<SummaryDto> GetSummaryAsync();
        Task<List<RecentApplicationDto>> GetRecentApplicationsAsync();
        Task<VacancyStatisticsDto> GetVacancyStatisticsAsync();
    }
}