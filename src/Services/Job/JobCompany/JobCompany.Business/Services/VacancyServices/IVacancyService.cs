﻿using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;

namespace JobCompany.Business.Services.VacancyServices
{
    public interface IVacancyService
    {
        Task CreateVacancyAsync(
            CreateVacancyDto vacancyDto,
            ICollection<CreateNumberDto>? numberDtos
        );
        Task UpdateVacancyAsync(
            UpdateVacancyDto vacancyDto,
            ICollection<UpdateNumberDto>? numberDtos
        );
        Task DeleteAsync(List<string> ids);
        Task<List<VacancyGetAllDto>> GetAllOwnVacanciesAsync(
            string? titleName,
            string? categoryId,
            string? countryId,
            string? cityId,
            bool? IsActive,
            decimal? minSalary,
            decimal? maxSalary,
            int skip = 1,
            int take = 6
        );
        Task<List<VacancyListDtoForAppDto>> GetAllVacanciesForAppAsync();
        Task<VacancyGetByIdDto> GetByIdVacancyAsync(string id);
        Task<ICollection<VacancyGetAllDto>> GetAllVacanciesAsync(
            string? titleName,
            string? categoryId,
            string? countryId,
            string? cityId,
            decimal? minSalary,
            decimal? maxSalary,
            int skip = 1,
            int take = 9
        );
        Task<ICollection<VacancyGetByCompanyIdDto>> GetVacancyByCompanyIdAsync(
            string companyId,
            int skip = 1,
            int take = 9
        );
    }
}
