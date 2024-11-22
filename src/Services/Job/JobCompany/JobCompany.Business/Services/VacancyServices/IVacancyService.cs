using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;

namespace JobCompany.Business.Services.VacancyServices
{
    public interface IVacancyService
    {
        Task CreateVacancyAsync(CreateVacancyDto vacancyDto, ICollection<CreateNumberDto>? numberDtos);
        Task UpdateVacancyAsync(UpdateVacancyDto vacancyDto, ICollection<UpdateNumberDto>? numberDtos);
        Task DeleteAsync(List<string> ids);
        Task<List<VacancyGetAllDto>> GetAllVacanciesAsync();
        Task<List<VacancyListDtoForAppDto>> GetAllVacanciesForAppAsync();
        Task<VacancyGetByIdDto> GetByIdVacancyAsync(string id);
        Task<ICollection<VacancyGetByCompanyIdDto>> GetVacancyByCompanyIdAsync(string companyId);
    }
}