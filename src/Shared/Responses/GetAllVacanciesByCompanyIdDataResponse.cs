using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Dtos.VacancyDtos;

namespace Shared.Responses
{
    public class GetAllVacanciesByCompanyIdDataResponse
    {
        public ICollection<AllVacanyDto> Vacancies { get; set; }
    }
}