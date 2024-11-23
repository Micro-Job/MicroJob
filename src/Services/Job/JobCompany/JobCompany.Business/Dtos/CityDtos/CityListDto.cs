using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.CityDtos
{
    public record CityListDto
    {
        public string CityName { get; set; }
        public Guid CountryId { get; set; }
    }
}