using Shared.Dtos.VacancyDtos;
using SharedLibrary.Enums;
using SharedLibrary.Responses;

namespace Job.Business.Services.Vacancy
{
    public interface IVacancyService
    {
        Task ToggleSaveVacancyAsync(string vacancyId);
        Task<GetUserSavedVacanciesResponse> GetAllSavedVacancyAsync(int skip, int take);
        Task<PaginatedVacancyDto> GetOtherVacanciesByCompanyAsync(
            string companyId,
            string currentVacancyId,
            int skip = 1,
            int take = 6
        );
        Task<GetVacancyInfoResponse> GetVacancyInfoAsync(Guid vacancyId);
        Task<PaginatedVacancyDto> GetAllVacanciesAsync(
            string? titleName,
            string? categoryId,
            string? countryId,
            string? cityId,
            bool? IsActive,
            decimal? minSalary,
            decimal? maxSalary,
            string? companyId,
            WorkType? workType,
            WorkStyle? workStyle,
            int skip = 1,
            int take = 6
        );
        Task<PaginatedSimilarVacancyDto> SimilarVacanciesAsync(string vacancyId);
        Task<ICollection<AllVacanyDto>> GetAllVacanciesByCompanyId(string companyId);
    }
}