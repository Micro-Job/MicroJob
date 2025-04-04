using FluentValidation;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.CertificateDtos
{
    public record CertificateUpdateDto
    {
        public string? Id { get; set; }
        public string? CertificateName { get; set; }
        public string? GivenOrganization { get; set; }
        public IFormFile? CertificateFile { get; set; }
    }

    public class CertificateUpdateDtoValidator : AbstractValidator<CertificateUpdateDto>
    {
        public CertificateUpdateDtoValidator()
        {
            RuleFor(x => x.CertificateName)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Length(1, 100).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"));

            RuleFor(x => x.GivenOrganization)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Length(1, 100).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"));

            RuleFor(x => x.CertificateFile)
                .NotNull().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
        }
    }
}
