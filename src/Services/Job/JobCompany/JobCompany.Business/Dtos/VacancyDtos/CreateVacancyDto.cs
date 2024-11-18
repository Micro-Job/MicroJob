using FluentValidation;
using JobCompany.Core.Enums;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.VacancyDtos
{
    public record CreateVacancyDto
    {
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public IFormFile? CompanyLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public string? Email { get; set; }
        public WorkType? WorkType { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }
        public Military Military { get; set; }
        public Driver Driver { get; set; }
        public FamilySituation Family { get; set; }
        public Citizenship Citizenship { get; set; }
        //public Guid? VacancyTestId { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class CreateVacancyDtoValidator : AbstractValidator<CreateVacancyDto>
    {
        public CreateVacancyDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name cannot be empty.")
                .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(150).WithMessage("Title must not exceed 150 characters.");

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
                .MaximumLength(200).WithMessage("Location must not exceed 200 characters.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("Country ID cannot be empty.");

            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("City ID cannot be empty.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Please enter a valid email address.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.WorkType)
                .IsInEnum().WithMessage("Invalid work type.");

            RuleFor(x => x.MainSalary)
                .GreaterThanOrEqualTo(0).WithMessage("Main salary cannot be negative.")
                .LessThanOrEqualTo(x => x.MaxSalary)
                .When(x => x.MaxSalary.HasValue)
                .WithMessage("Main salary cannot exceed the maximum salary.");

            RuleFor(x => x.MaxSalary)
                .GreaterThanOrEqualTo(0).WithMessage("Maximum salary cannot be negative.");

            RuleFor(x => x.Requirement)
                .NotEmpty().WithMessage("Requirement cannot be empty.")
                .MaximumLength(1000).WithMessage("Requirement must not exceed 1000 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description cannot be empty.")
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value.");

            RuleFor(x => x.Military)
                .IsInEnum().WithMessage("Invalid military status.");

            RuleFor(x => x.Driver)
                .IsInEnum().WithMessage("Invalid driver status.");

            RuleFor(x => x.Family)
                .IsInEnum().WithMessage("Invalid family situation.");

            RuleFor(x => x.Citizenship)
                .IsInEnum().WithMessage("Invalid citizenship status.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID cannot be empty.");
        }
    }
}