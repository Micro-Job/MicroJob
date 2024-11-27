using FluentValidation;

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
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(dto => dto.VacancyId)
                .NotEmpty().WithMessage("VacancyId is required.");
        }
    }
}