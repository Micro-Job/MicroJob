using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace JobCompany.Business.Dtos.CountryDtos
{
    public record CreateCountryDto
    {
        public string CountryName { get; set; }
    }

    public class CreateCountryDtoValidator : AbstractValidator<CreateCountryDto>
    {
        public CreateCountryDtoValidator()
        {
            RuleFor(x => x.CountryName)
                .NotEmpty()
                .WithMessage("Country name cannot be empty");
        }
    }
}