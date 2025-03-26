using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.StatusDtos;
using Shared.Responses;
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Enums;

namespace JobCompany.Business.Services.ApplicationServices
{
    public interface IApplicationService
    {
        Task RemoveApplicationAsync(string applicationId);
        Task ChangeApplicationStatusAsync(string applicationId, string statusId);
        Task<ApplicationGetByIdDto> GetApplicationByIdAsync(string applicationId);
        Task<ICollection<ApplicationInfoListDto>> GetAllApplicationAsync(int skip = 1,int take = 9);
        Task<DataListDto<AllApplicationListDto>> GetAllApplicationsListAsync(
        Gender gender, Guid statusId, Guid vacancyId, Guid skillId, int skip = 1, int take = 10);

        Task CreateUserApplicationAsync(string vacancyId);
        Task<PaginatedApplicationDto> GetUserApplicationsAsync(int skip, int take);
        Task<ApplicationDetailDto> GetUserApplicationByIdAsync(string applicationId);
    }
}