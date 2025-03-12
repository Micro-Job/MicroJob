using JobCompany.Business.Dtos.StatusDtos;

namespace JobCompany.Business.Services.StatusServices
{
    public interface IStatusService
    {
        Task CreateStatusAsync(CreateStatusDto dto);
        Task DeleteStatusAsync(string statusId);
        Task<List<StatusListDto>> GetAllStatusesAsync();
    }
}