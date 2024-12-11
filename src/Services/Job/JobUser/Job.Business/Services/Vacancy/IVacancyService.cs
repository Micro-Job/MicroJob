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
    }
}