using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.VacancyDtos
{
    public record VacancyListDtoForAppDto
    {
        public Guid VacancyId { get; set; }
        public string VacancyName { get; set; }
    }
}
