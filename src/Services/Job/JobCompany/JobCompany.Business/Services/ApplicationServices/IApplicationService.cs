using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Dtos.StatusDtos;
using Shared.Responses;
using SharedLibrary.Dtos.ApplicationDtos;

namespace JobCompany.Business.Services.ApplicationServices
{
    public interface IApplicationService
    {
        Task RemoveApplicationAsync(string applicationId);
        Task ChangeApplicationStatusAsync(string applicationId, string statusId);
        Task<List<StatusListDtoWithApps>> GetAllApplicationWithStatusAsync(string vacancyId);
        Task<List<ApplicationUserListDto>> GetUserApplicationAsync(int skip = 1, int take = 9);
        Task<ApplicationGetByIdDto> GetApplicationByIdAsync(string applicationId);
        Task<ICollection<ApplicationInfoListDto>> GetAllApplicationAsync(
            int skip = 1,
            int take = 9
        );
        Task<ICollection<AllApplicationListDto>> GetAllApplicationsListAsync(
            int skip = 1,
            int take = 10
        );

        Task CreateUserApplicationAsync(string vacancyId);
        Task<PaginatedApplicationDto> GetUserApplicationsAsync(int skip, int take);
        Task<GetApplicationDetailResponse> GetUserApplicationByIdAsync(string applicationId);
    }
}