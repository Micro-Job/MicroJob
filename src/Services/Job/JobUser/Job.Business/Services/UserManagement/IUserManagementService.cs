using Job.Business.Dtos.Common;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Dtos.UserDtos;
using SharedLibrary.Enums;
using SharedLibrary.Responses;

namespace Job.Business.Services.UserManagement;

public interface IUserManagementService
{
    Task<DataListDto<GetUsersDataForAdminResponse>> GetAllUsersAsync(UserRole userRole, string? searchTerm, int pageIndex = 1, int pageSize = 10);
    Task<UserPersonalInfoDto> GetPersonalInfoAsync(string userId);
    Task<ResumeDetailItemDto> GetResumeDetailAsync(string userId);
}
