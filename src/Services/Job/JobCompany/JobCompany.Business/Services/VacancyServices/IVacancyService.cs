using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using SharedLibrary.Enums;

namespace JobCompany.Business.Services.VacancyServices
{
    public interface IVacancyService
    {
        Task CreateVacancyAsync(CreateVacancyDto vacancyDto, ICollection<CreateNumberDto>? numberDtos);
        Task UpdateVacancyAsync(UpdateVacancyDto vacancyDto, ICollection<UpdateNumberDto>? numberDtos);
        Task DeleteAsync(List<string> ids);
        Task<List<VacancyGetAllDto>> GetAllOwnVacanciesAsync(string? titleName, List<string>? categoryIds, List<string>? countryIds, List<string>? cityIds, VacancyStatus? IsActive, decimal? minSalary, decimal? maxSalary, List<byte>? workStyles, List<byte>? workTypes, List<Guid>? skillIds, int skip = 1, int take = 6);
        Task<List<VacancyListDtoForAppDto>> GetAllVacanciesForAppAsync();
        Task<VacancyGetByIdDto> GetByIdVacancyAsync(string id);
        Task<VacancyDetailsDto> GetVacancyDetailsAsync(Guid id);
        Task<DataListDto<VacancyGetAllDto>> GetAllVacanciesAsync(string? titleName, List<string>? categoryIds, List<string>? countryIds, List<string>? cityIds, decimal? minSalary, decimal? maxSalary, List<string>? companyIds, List<byte>? workStyles, List<byte>? workTypes, List<Guid>? skillIds, int skip = 1, int take = 9);
        Task<DataListDto<VacancyGetByCompanyIdDto>> GetVacanciesByCompanyIdAsync(string companyId, Guid? vacancyId, int skip = 1, int take = 9);

        Task ToggleSaveVacancyAsync(string vacancyId);
        Task<List<VacancyGetAllDto>> SimilarVacanciesAsync(string vacancyId, int take = 6);

        Task<DataListDto<VacancyGetAllDto>> GetAllSavedVacancyAsync(int skip, int take, string? vacancyName);

        Task DeleteVacancyAsync(Guid vacancyId);


        Task TogglePauseVacancyAsync(Guid vacancyId);

        Task ActivateVacancyAsync(Guid vacancyId);

    }
}
