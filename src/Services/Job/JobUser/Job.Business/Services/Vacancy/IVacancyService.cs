﻿using Shared.Dtos.VacancyDtos;
using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Responses;

namespace Job.Business.Services.Vacancy
{
    public interface IVacancyService
    {
        Task ToggleSaveVacancyAsync(string vacancyId);
        Task<List<VacancyResponse>> GetAllSavedVacancyAsync(int skip, int take);
        Task<ICollection<AllVacanyDto>> GetOtherVacanciesByCompanyAsync(
            string companyId,
            string currentVacancyId,
            int skip = 1,
            int take = 6
        );
        Task<GetVacancyInfoResponse> GetVacancyInfoAsync(Guid vacancyId);
        Task<ICollection<AllVacanyDto>> GetAllVacanciesAsync(
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
        Task<ICollection<SimilarVacancyDto>> SimilarVacanciesAsync(string vacancyId);
        Task<ICollection<AllVacanyDto>> GetAllVacanciesByCompanyId(string companyId);
    }
}
