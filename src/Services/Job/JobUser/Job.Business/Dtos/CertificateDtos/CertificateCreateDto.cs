using FluentValidation;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.CertificateDtos
{
    public class CertificateCreateDto
    {
        public string CertificateName { get; set; }
        public string GivenOrganization { get; set; }
        public IFormFile CertificateFile { get; set; }
    }

    public class CertificateCreateDtoValidator : AbstractValidator<CertificateCreateDto>
    {
        private const long _maxFileSizeBytes = 20 * 1024 * 1024; 

        public CertificateCreateDtoValidator()
        {
            RuleFor(x => x.CertificateName)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Length(1, 100).WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.CertificateName?.Length ?? 0, 100));

            RuleFor(x => x.GivenOrganization)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Length(1, 100).WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.GivenOrganization?.Length ?? 0, 100));

            RuleFor(x => x.CertificateFile)
            .NotNull().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
            .Custom((file, context) =>
            {
                if (file != null)
                {
                    var fileSizeMb = file.Length / (1024.0 * 1024.0); 
                    var maxAllowedSizeMb = _maxFileSizeBytes / (1024.0 * 1024.0);
                    if (file.Length > _maxFileSizeBytes)
                    {
                        var errorMessage = MessageHelper.GetMessage(
                            "FILE_SIZE_EXCEEDED",
                            fileSizeMb.ToString("F2"),
                            maxAllowedSizeMb.ToString("F2")
                        );
                        context.AddFailure(errorMessage);
                    }
        }
        });
        }
    }
}
