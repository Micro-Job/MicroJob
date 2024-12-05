using SharedLibrary.Dtos.VacancyDtos;

namespace SharedLibrary.Responses
{
    public class UserVacanciesResponse
    {
        public List<VacancyDto> Vacancies { get; set; }
    }
}