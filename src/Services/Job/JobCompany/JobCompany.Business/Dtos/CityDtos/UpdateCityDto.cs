using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.CityDtos
{
    public record UpdateCityDto
    {
        public string? CityName { get; set; }
    }
}