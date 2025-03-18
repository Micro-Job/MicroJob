using FluentValidation;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public record ApplicationCreateDto
    {
        public string UserId { get; set; }
        public string VacancyId { get; set; }
    }

    public class ApplicationCreateDtoValidator : AbstractValidator<ApplicationCreateDto>
    {
        public ApplicationCreateDtoValidator()
        {
            RuleFor(dto => dto.UserId)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));

            RuleFor(dto => dto.VacancyId)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
        }
    }
}