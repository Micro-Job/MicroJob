using FluentValidation;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Helpers;

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
        public string? Email { get; set; }
        public IFormFile? CompanyLogo { get; set; }
    }
    public class CompanyUpdateDtoValidator : AbstractValidator<CompanyUpdateDto>
    {
        public CompanyUpdateDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(100).WithMessage(x =>
                MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.CompanyName?.Length ?? 0, 100))
                .When(x => x.CompanyName != null);

            RuleFor(x => x.CompanyInformation)
                .MaximumLength(500).WithMessage(x =>
                MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.CompanyInformation?.Length ?? 0, 500))
                .When(x => x.CompanyInformation != null);

            RuleFor(x => x.CompanyLocation)
                .MaximumLength(200).WithMessage(x =>
                MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.CompanyLocation?.Length ?? 0, 200))
                .When(x => x.CompanyLocation != null);

            RuleFor(x => x.CreatedDate)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .LessThanOrEqualTo(DateTime.Now).WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"))
                .When(x => x.CreatedDate.HasValue);

            RuleFor(x => x.WebLink)
                .Matches(@"^(http|https)://[^\s/$.?#].[^\s]*$")
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"))
                .When(x => x.WebLink != null);

            RuleFor(x => x.EmployeeCount)
                .GreaterThanOrEqualTo(0).WithMessage(MessageHelper.GetMessage("GREATER_THAN_OR_EQUAL_TO_ZERO"))
                .When(x => x.EmployeeCount.HasValue);

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"))
                .When(x => x.Email != null);
        }
    }
}