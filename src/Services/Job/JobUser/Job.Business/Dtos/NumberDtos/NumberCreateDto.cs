using System;
using FluentValidation;

namespace Job.Business.Dtos.NumberDtos
{
    public record NumberCreateDto
    {
        public string PersonId { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class NumberCreateDtoValidator : AbstractValidator<NumberCreateDto>
    {
        public NumberCreateDtoValidator()
        {
            RuleFor(x => x.PersonId)
                .NotEmpty().WithMessage("PersonId is required.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{7,15}$")
                .WithMessage("Phone number must be a valid format and between 7 and 15 digits.");
        }
    }
}
