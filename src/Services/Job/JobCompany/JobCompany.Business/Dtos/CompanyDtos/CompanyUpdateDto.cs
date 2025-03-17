using FluentValidation;
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
    }
    public class CompanyUpdateDtoValidator : AbstractValidator<CompanyUpdateDto>
    {
        public CompanyUpdateDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(100).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"))
                .When(x => x.CompanyName != null);

            RuleFor(x => x.CompanyInformation)
                .MaximumLength(500).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_500"))
                .When(x => x.CompanyInformation != null);

            RuleFor(x => x.CompanyLocation)
                .MaximumLength(200).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_200"))
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
        }
    }
}