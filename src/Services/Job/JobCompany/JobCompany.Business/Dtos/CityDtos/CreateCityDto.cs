using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace JobCompany.Business.Dtos.CityDtos
{
    public class CreateCityDto
    {
        public string CityName { get; set; }
        public string CountryId { get; set; }
    }

    public class CreateCityDtoValidator: AbstractValidator<CreateCityDto>
    {
        public CreateCityDtoValidator()
        {
            RuleFor(x => x.CityName).NotEmpty().WithMessage("City name is required");
            RuleFor(x => x.CountryId).NotEmpty().WithMessage("Country id is required");
        }
    }
}