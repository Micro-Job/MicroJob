﻿using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Dtos.StatusDtos;
using Shared.Responses;

namespace JobCompany.Business.Services.ApplicationServices
{
    public interface IApplicationService
    {
        Task CreateApplicationAsync(ApplicationCreateDto dto);
        Task RemoveApplicationAsync(string applicationId);
        Task ChangeApplicationStatusAsync(string applicationId, string statusId);
        Task<List<StatusListDtoWithApps>> GetAllApplicationWithStatusAsync(string vacancyId);
        Task GetAllApplicationWithStatusAsync(string vacancyId, string statusId, int skip = 1, int take = 5);
        Task<List<ApplicationUserListDto>> GetUserApplicationAsync(int skip = 1, int take = 9);
        Task<ApplicationGetByIdDto> GetApplicationByIdAsync(string applicationId);
        Task<GetUsersDataResponse> GetUserDataResponseAsync(List<Guid> userIds);
        Task<GetResumesDataResponse> GetResumeDataResponseAsync(List<Guid> userIds);
        Task<ICollection<ApplicationInfoListDto>> GetAllApplicationAsync(int skip = 1, int take = 9);
    }
}