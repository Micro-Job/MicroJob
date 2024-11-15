using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;

namespace JobCompany.Business.Services.VacancyServices
{
    public interface IVacancyService
    {
        Task CreateVacancyAsync(CreateVacancyDto vacancyDto, ICollection<CreateNumberDto>? numberDtos);
        Task UpdateVacancyAsync(UpdateVacancyDto vacancyDto, ICollection<UpdateNumberDto>? numberDtos);
        Task DeleteAsync(string id);
        Task<List<VacancyGetAllDto>> GetAllVacanciesAsync();
        Task<VacancyGetByIdDto> GetByIdVacancyAsync(string id);
    }
}