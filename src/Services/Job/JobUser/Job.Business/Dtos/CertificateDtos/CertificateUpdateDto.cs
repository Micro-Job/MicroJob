using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Dtos.CertificateDtos
{
    public record CertificateUpdateDto
    {
        public string Id { get; set; }
        public string CertificateName { get; set; }
        public string GivenOrganization { get; set; }
        public IFormFile CertificateFile { get; set; }
    }

    public class CertificateUpdateDtoValidator : AbstractValidator<CertificateUpdateDto>
    {
        public CertificateUpdateDtoValidator()
        {
            RuleFor(x => x.CertificateName)
                .NotEmpty()
                .WithMessage("Certificate name cannot be empty.")
                .Length(1, 100)
                .WithMessage("Certificate name must be between 1 and 100 characters.");

            RuleFor(x => x.GivenOrganization)
                .NotEmpty()
                .WithMessage("Given organization cannot be empty.")
                .Length(1, 100)
                .WithMessage("Given organization must be between 1 and 100 characters.");

            RuleFor(x => x.CertificateFile)
                .NotEmpty()
                .WithMessage("Certificate file cannot be empty.");
        }
    }
}
