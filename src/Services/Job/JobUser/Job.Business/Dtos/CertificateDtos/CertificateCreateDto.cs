using FluentValidation;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.CertificateDtos
{
    public record CertificateCreateDto
    {
        public string CertificateName { get; set; }
        public string GivenOrganization { get; set; }
        public IFormFile CertificateFile { get; set; }
    }

    public class CertificateCreateDtoValidator : AbstractValidator<CertificateCreateDto>
    {
        public CertificateCreateDtoValidator()
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
