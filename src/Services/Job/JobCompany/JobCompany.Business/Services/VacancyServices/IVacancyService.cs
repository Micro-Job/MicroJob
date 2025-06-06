﻿using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using SharedLibrary.Enums;

namespace JobCompany.Business.Services.VacancyServices
{
    public interface IVacancyService
    {
        Task CreateVacancyAsync(CreateVacancyDto vacancyDto,ICollection<CreateNumberDto>? numberDtos);
        Task UpdateVacancyAsync(UpdateVacancyDto vacancyDto,ICollection<UpdateNumberDto>? numberDtos);
        Task DeleteAsync(List<string> ids);
        Task<List<VacancyGetAllDto>> GetAllOwnVacanciesAsync(string? titleName,string? categoryId,string? countryId,string? cityId,VacancyStatus? IsActive,decimal? minSalary,decimal? maxSalary,int skip = 1,int take = 6);
        Task<List<VacancyListDtoForAppDto>> GetAllVacanciesForAppAsync();
        Task<VacancyGetByIdDto> GetByIdVacancyAsync(string id);
        Task<DataListDto<VacancyGetAllDto>> GetAllVacanciesAsync(string? titleName,string? categoryId,string? countryId,string? cityId,decimal? minSalary,decimal? maxSalary, string? companyId, byte? workStyle, byte? workType, int skip = 1,int take = 9);
        Task<DataListDto<VacancyGetByCompanyIdDto>> GetVacanciesByCompanyIdAsync(string companyId,int skip = 1,int take = 9);

        Task ToggleSaveVacancyAsync(string vacancyId);
        Task<List<VacancyGetAllDto>> SimilarVacanciesAsync(string vacancyId, int take = 6);

        Task<DataListDto<VacancyGetAllDto>> GetAllSavedVacancyAsync(int skip, int take);
    }
}
