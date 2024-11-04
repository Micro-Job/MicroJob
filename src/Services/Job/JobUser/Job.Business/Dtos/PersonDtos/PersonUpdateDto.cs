using System;
using FluentValidation;
using Job.Business.Validators;
using Job.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Dtos.PersonDtos
{
    public record PersonUpdateDto
    {
        public string Id { get; set; }
        public string? FatherName { get; set; }
        public IFormFile? UserPhoto { get; set; }
        public bool? IsDriver { get; set; }
        public bool? IsMarried { get; set; }
        public bool? IsCitizen { get; set; }
        public Gender? Gender { get; set; }
        public string? Adress { get; set; }
        public DateTime? BirthDay { get; set; }
    }

    public class PersonUpdateDtoValidator : AbstractValidator<PersonUpdateDto>
    {
        public PersonUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            When(c => c.FatherName != null, () =>
            {
                RuleFor(c => c.FatherName)
                    .NotEmpty().WithMessage("Father name cannot be empty.");
            });

            When(c => c.UserPhoto != null, () =>
            {
                RuleFor(c => c.UserPhoto)
                    .SetValidator(new FileValidator())
                    .WithMessage("User Photo must be valid.");
            });

            When(c => c.BirthDay.HasValue, () =>
            {
                RuleFor(x => x.BirthDay)
                    .LessThan(DateTime.Today).WithMessage("Birth date must be in the past.");
            });

            When(c => c.Adress != null, () =>
            {
                RuleFor(x => x.Adress)
                    .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");
            });

            When(c => c.Gender.HasValue, () =>
            {
                RuleFor(x => x.Gender)
                    .IsInEnum().WithMessage("Gender must be a valid value.");
            });
        }
    }
}
