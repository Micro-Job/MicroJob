using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.NumberDtos
{
    public record CreateNumberDto
    {
        public string? PhoneNumber { get; set; }
    }

    public class CreateNumberValidator : AbstractValidator<CreateNumberDto>
    {
        public CreateNumberValidator()
        {
            RuleFor(x=>x.PhoneNumber)
                .Matches(@"^(?:\+\d{1,3})?\d{1,4}\d{7,10}$")
                .WithMessage("Telefon nömrəsi doğru formatda deyil");
        }
    }
}
