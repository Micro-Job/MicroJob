using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Job.Business.Validators;
using Job.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Dtos.PersonDtos
{
    public record PersonCreateDto
    {
        public string UserId { get; set; }
        public string FatherName { get; set; }
        public IFormFile? UserPhoto { get; set; }
        public bool IsDriver { get; set; }
        public bool IsMarried { get; set; }
        public bool IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }
    }

    public class PersonCreateDtoValidator : AbstractValidator<PersonCreateDto>
    {
        public PersonCreateDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.FatherName)
                .NotEmpty().WithMessage("Father name is required.");

            When(c => c.UserPhoto != null, () =>
            {
                RuleFor(c => c.UserPhoto)
                    .SetValidator(new FileValidator())
                    .WithMessage("User Photo must be a valid");
            });
            RuleFor(x => x.BirthDay)
                .LessThan(DateTime.Today).WithMessage("Birth date must be in the past.");

            RuleFor(x => x.Adress)
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");
            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Gender must be a valid value.");
        }
    }
}