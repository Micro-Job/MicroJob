using FluentValidation;

namespace JobCompany.Business.Dtos.CompanyDtos
{
    public record CompanyUpdateDto
    {
        public string? CompanyName { get; set; }
        public string? CompanyInformation { get; set; }
        public string? CompanyLocation { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? WebLink { get; set; }
        public int? EmployeeCount { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? CityId { get; set; }
    }
    public class CompanyUpdateDtoValidator : AbstractValidator<CompanyUpdateDto>
    {
        public CompanyUpdateDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.")
                .When(x => x.CompanyName != null);

            RuleFor(x => x.CompanyInformation)
                .MaximumLength(500).WithMessage("Company information must not exceed 500 characters.")
                .When(x => x.CompanyInformation != null);

            RuleFor(x => x.CompanyLocation)
                .MaximumLength(200).WithMessage("Company location must not exceed 200 characters.")
                .When(x => x.CompanyLocation != null);

            RuleFor(x => x.CreatedDate)
                .NotEmpty().WithMessage("CreatedDate is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("CreatedDate cannot be in the future.")
                .When(x => x.CreatedDate.HasValue);

            RuleFor(x => x.WebLink)
                .Matches(@"^(http|https)://[^\s/$.?#].[^\s]*$")
                .WithMessage("WebLink must be a valid URL.")
                .When(x => x.WebLink != null);

            RuleFor(x => x.EmployeeCount)
                .GreaterThanOrEqualTo(0).WithMessage("Employee count must be at least 0")
                .When(x => x.EmployeeCount.HasValue);
        }
    }
}