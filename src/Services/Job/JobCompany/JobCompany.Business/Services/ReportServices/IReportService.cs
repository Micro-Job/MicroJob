using JobCompany.Business.Dtos.ReportDtos;

namespace JobCompany.Business.Services.ReportServices
{
    public interface IReportService
    {
        Task<SummaryDto> GetSummaryAsync();
        Task<List<RecentApplicationDto>> GetRecentApplicationsAsync();
    }
}