using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Responses;

namespace Job.Business.Services.Vacancy
{
    public interface IVacancyService
    {
        Task ToggleSaveVacancyAsync(string vacancyId);
        Task<GetUserSavedVacanciesResponse> GetAllSavedVacancyAsync();
        Task<GetUserSavedVacanciesResponse> GetUserSavedVacancyDataAsync(List<Guid> VacancyIds);
        Task<ICollection<CompanyDto>> GetAllCompaniesAsync();
    }
}