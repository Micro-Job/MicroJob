using FluentValidation;
using JobCompany.Core.Enums;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using SharedLibrary.Enums;

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
        public string Requirement { get; set; }
        public string Description { get; set; }
        public SharedLibrary.Enums.Gender Gender { get; set; }
        public Military Military { get; set; }
        public Driver Driver { get; set; }
        public SharedLibrary.Enums.FamilySituation Family { get; set; }
        public Citizenship Citizenship { get; set; }

        //public Guid? VacancyTestId { get; set; }
        public string? CategoryId { get; set; }
    }

    public class UpdateVacancyDtoValidator : AbstractValidator<UpdateVacancyDto>
    {
        public UpdateVacancyDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Vacancy ID cannot be empty.")
                .Must(IsValidGuid)
                .WithMessage("Vacancy ID must be a valid GUID.");

            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID cannot be empty.")
                .Must(IsValidGuid)
                .WithMessage("Company ID must be a valid GUID.");

            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .WithMessage("Company name cannot be empty.")
                .MaximumLength(100)
                .WithMessage("Company name must not exceed 100 characters.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title cannot be empty.")
                .MaximumLength(150)
                .WithMessage("Title must not exceed 150 characters.");

            RuleFor(x => x.CompanyLogo)
                .Must(file => file == null || file.ContentType.StartsWith("image/"))
                .WithMessage("The company logo must be an image file.");

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .When(x => x.EndDate.HasValue)
                .WithMessage("Start date must be earlier than the end date.")
                .GreaterThanOrEqualTo(DateTime.Now)
                .WithMessage("Start date cannot be in the past.");

            RuleFor(x => x.EndDate)
                .GreaterThan(DateTime.Now)
                .When(x => x.EndDate.HasValue)
                .WithMessage("End date cannot be in the past.");

            RuleFor(x => x.Location)
                .MaximumLength(200)
                .WithMessage("Location must not exceed 200 characters.");

            RuleFor(x => x.CountryId)
                .Must(IsValidGuid)
                .WithMessage("Country ID must be a valid GUID.")
                .When(x => !string.IsNullOrWhiteSpace(x.CountryId));

            RuleFor(x => x.CityId)
                .Must(IsValidGuid)
                .WithMessage("City ID must be a valid GUID.")
                .When(x => !string.IsNullOrWhiteSpace(x.CityId));

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Please enter a valid email address.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.WorkType).IsInEnum().WithMessage("Invalid work type.");

            RuleFor(x => x.MainSalary)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Main salary cannot be negative.")
                .LessThanOrEqualTo(x => x.MaxSalary)
                .When(x => x.MaxSalary.HasValue)
                .WithMessage("Main salary cannot exceed the maximum salary.");

            RuleFor(x => x.MaxSalary)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Maximum salary cannot be negative.");

            RuleFor(x => x.Requirement)
                .NotEmpty()
                .WithMessage("Requirement cannot be empty.")
                .MaximumLength(1000)
                .WithMessage("Requirement must not exceed 1000 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description cannot be empty.")
                .MaximumLength(2000)
                .WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.Gender).IsInEnum().WithMessage("Invalid gender value.");

            RuleFor(x => x.Military).IsInEnum().WithMessage("Invalid military status.");

            RuleFor(x => x.Driver).IsInEnum().WithMessage("Invalid driver status.");

            RuleFor(x => x.Family).IsInEnum().WithMessage("Invalid family situation.");

            RuleFor(x => x.Citizenship).IsInEnum().WithMessage("Invalid citizenship status.");

            RuleFor(x => x.CategoryId)
                .Must(IsValidGuid)
                .WithMessage("Category ID must be a valid GUID.")
                .When(x => !string.IsNullOrWhiteSpace(x.CategoryId));
        }

        private bool IsValidGuid(string? id)
        {
            return Guid.TryParse(id, out _);
        }
    }
}
