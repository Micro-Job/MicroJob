using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Dtos.VacancyDtos;
using SharedLibrary.Dtos.VacancyDtos;

namespace Shared.Responses
{
    public class GetAllVacanciesResponse
    {
        public ICollection<AllVacanyDto> Vacancies { get; set; }
    }
}