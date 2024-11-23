using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace JobCompany.Business.Dtos.CountryDtos
{
    public record UpdateCountryDto
    {
        public string? CountryName { get; set; }
    }
}