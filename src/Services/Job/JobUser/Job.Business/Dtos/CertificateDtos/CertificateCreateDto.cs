using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Job.Business.Dtos.CertificateDtos
{
    public record CertificateCreateDto
    {
        public string CertificateName { get; set; }
        public string GivenOrganization { get; set; }
        public string CertificateFile { get; set; }
    }

    public class CertificateCreateDtoValidator : AbstractValidator<CertificateCreateDto>
    {
        public CertificateCreateDtoValidator()
        {
            RuleFor(x => x.CertificateName)
                .NotEmpty().WithMessage("Certificate name cannot be empty.")
                .Length(1, 100).WithMessage("Certificate name must be between 1 and 100 characters.");

            RuleFor(x => x.GivenOrganization)
                .NotEmpty().WithMessage("Given organization cannot be empty.")
                .Length(1, 100).WithMessage("Given organization must be between 1 and 100 characters.");

            RuleFor(x => x.CertificateFile)
                .NotEmpty().WithMessage("Certificate file cannot be empty.");
        }
    }
}