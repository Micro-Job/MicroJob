using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.NumberDtos;

namespace JobCompany.Business.Dtos.CompanyDtos
{
    public record UpdateCompanyRequest
    {
        public CompanyUpdateDto Dto { get; set; }
        public ICollection<UpdateNumberDto>? numbersDto {get;set;}
    }
}