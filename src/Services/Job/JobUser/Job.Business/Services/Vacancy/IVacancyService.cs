using Shared.Dtos.VacancyDtos;
using Shared.Responses;
using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Dtos.VacancyDtos;
using SharedLibrary.Responses;

namespace Job.Business.Services.Vacancy
{
    public interface IVacancyService
    {
        Task ToggleSaveVacancyAsync(string vacancyId);
        Task<GetUserSavedVacanciesResponse> GetAllSavedVacancyAsync();
        Task<GetUserSavedVacanciesResponse> GetUserSavedVacancyDataAsync(List<Guid> VacancyIds);
        Task<ICollection<CompanyDto>> GetAllCompaniesAsync();
        Task<List<VacancyDto>> GetAllUserVacanciesAsync();
        Task<GetVacancyInfoResponse> GetVacancyInfoAsync(Guid vacancyId);
        Task<ICollection<AllVacanyDto>> GetAllVacanciesAsync(string? titleName, string? categoryId, string? countryId, string? cityId, bool? IsActive, decimal? minSalary, decimal? maxSalary, int skip = 1, int take = 6);
        Task<List<SimilarVacancyResponse>> SimilarVacanciesAsync(string vacancyId, string userId);
    }
}