using FluentValidation;
using JobCompany.Business.Dtos.SkillDtos;
using JobCompany.Core.Enums;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.VacancyDtos
{
    public record UpdateVacancyDto
    {
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public IFormFile? CompanyLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? CountryId { get; set; }
        public string? CityId { get; set; }
        public string? Email { get; set; }
        public WorkType? WorkType { get; set; }
        public WorkStyle? WorkStyle { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public SalaryCurrencyType? SalaryCurrency { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }
        public Military Military { get; set; }
        public Driver Driver { get; set; }
        public FamilySituation Family { get; set; }
        public Citizenship Citizenship { get; set; }
        public Guid? ExamId { get; set; }
        public string? CategoryId { get; set; }
        public ICollection<SkillUpdateDto> Skills { get; set; } = [];
    }

    public class UpdateVacancyDtoValidator : AbstractValidator<UpdateVacancyDto>
    {
        public UpdateVacancyDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Must(IsValidGuid)
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Must(IsValidGuid)
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(100)
                .WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.CompanyName?.Length ?? 0, 100));

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(150)
                .WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.CompanyName?.Length ?? 0, 150));

            RuleFor(x => x.CompanyLogo)
                .Must(file => file == null || file.ContentType.StartsWith("image/"))
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .When(x => x.EndDate.HasValue)
                .WithMessage(MessageHelper.GetMessage("STARTDATE_MUST_BE_EARLİER_ENDATE"));

            RuleFor(x => x.EndDate)
                .GreaterThan(DateTime.Now)
                .When(x => x.EndDate.HasValue)
                .WithMessage(MessageHelper.GetMessage("END_DATE_MUST_NOT_BE_IN_THE_PAST"));

            RuleFor(x => x.Location)
                .MaximumLength(200)
                .WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.Location?.Length ?? 0, 200));

            RuleFor(x => x.CountryId)
                .Must(IsValidGuid)
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"))
                .When(x => !string.IsNullOrWhiteSpace(x.CountryId));

            RuleFor(x => x.CityId)
                .Must(IsValidGuid)
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"))
                .When(x => !string.IsNullOrWhiteSpace(x.CityId));

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"))
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.WorkType).IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.MainSalary)
                .GreaterThanOrEqualTo(0)
                .GreaterThanOrEqualTo(0).WithMessage(MessageHelper.GetMessage("GREATER_THAN_OR_EQUAL_TO_ZERO"))
                .LessThanOrEqualTo(x => x.MaxSalary)
                .When(x => x.MaxSalary.HasValue)
                .WithMessage(MessageHelper.GetMessage("MAIN_SALARY"));

            RuleFor(x => x.MaxSalary)
                .GreaterThanOrEqualTo(0).WithMessage(MessageHelper.GetMessage("GREATER_THAN_OR_EQUAL_TO_ZERO"));

            RuleFor(x => x.Requirement)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(8192)
                .WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.Requirement?.Length ?? 0, 8192));

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(8192)
                .WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.Description?.Length ?? 0, 8192));

            RuleFor(x => x.Gender).IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.Military).IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.Driver).IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.Family).IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.Citizenship).IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleFor(x => x.CategoryId)
                .Must(IsValidGuid)
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"))
                .When(x => !string.IsNullOrWhiteSpace(x.CategoryId));
        }

        private bool IsValidGuid(string? id)
        {
            return Guid.TryParse(id, out _);
        }
    }
}
