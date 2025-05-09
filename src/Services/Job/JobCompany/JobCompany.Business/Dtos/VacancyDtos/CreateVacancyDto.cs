﻿using FluentValidation;
using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Core.Enums;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.VacancyDtos
{
    public record CreateVacancyDto
    {
        public string Title { get; set; }
        public string? CompanyName { get; set; }
        public IFormFile? CompanyLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public string? Email { get; set; }
        public WorkType? WorkType { get; set; }
        public WorkStyle? WorkStyle { get; set; } 
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; } 
        public Military Military { get; set; } 
        public Driver Driver { get; set; } 
        public FamilySituation Family { get; set; } 
        public Citizenship Citizenship { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? ExamId { get; set; }
        public List<Guid>? SkillIds { get; set; }
    }

    public class CreateVacancyDtoValidator : AbstractValidator<CreateVacancyDto>
    {
        public CreateVacancyDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(150).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"));

            RuleFor(x => x.CompanyLogo)
                .Must(file => file == null || file.ContentType.StartsWith("image/"))
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .When(x => x.EndDate.HasValue)
                .WithMessage(MessageHelper.GetMessage("STARTDATE_MUST_BE_EARLİER_ENDATE"))
                .GreaterThanOrEqualTo(DateTime.Now)
                .WithMessage(MessageHelper.GetMessage("STARTDATE_MUST_BE_EARLİER_ENDATE"));

            RuleFor(x => x.EndDate)
                .GreaterThan(DateTime.Now)
                .When(x => x.EndDate.HasValue)
                .WithMessage(MessageHelper.GetMessage("END_DATE_MUST_NOT_BE_IN_THE_PAST"));

            RuleFor(x => x.Location)
                .MaximumLength(200).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_200"));

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));

            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"))
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.WorkType)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.MainSalary)
                .GreaterThanOrEqualTo(0).WithMessage(MessageHelper.GetMessage("GREATER_THAN_OR_EQUAL_TO_ZERO"))
                .LessThanOrEqualTo(x => x.MaxSalary)
                .When(x => x.MaxSalary.HasValue)
                .WithMessage(MessageHelper.GetMessage("MAIN_SALARY"));

            RuleFor(x => x.MaxSalary)
                .GreaterThanOrEqualTo(0).WithMessage(MessageHelper.GetMessage("GREATER_THAN_OR_EQUAL_TO_ZERO"));

            RuleFor(x => x.Requirement)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(1000).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_1000"));

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(1000).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_1000"));

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.Military)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.Driver)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.Family)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.Citizenship)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
        }
    }
}