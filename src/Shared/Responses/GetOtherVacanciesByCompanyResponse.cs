using Shared.Dtos.VacancyDtos;

namespace SharedLibrary.Responses;

public class GetOtherVacanciesByCompanyResponse
{
    public ICollection<AllVacanyDto>? Vacancies { get; set; }
}
