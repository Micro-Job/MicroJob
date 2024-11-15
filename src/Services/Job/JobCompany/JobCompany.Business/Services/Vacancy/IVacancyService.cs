using JobCompany.Business.Dtos.NumberDtos;

namespace JobCompany.Business.Services.Vacancy
{
    public interface IVacancyService
    {
        Task CreateVacancyAsync(CreateVacancyDto vacancyDto, ICollection<CreateNumberDto>? numberDto);
    }
}