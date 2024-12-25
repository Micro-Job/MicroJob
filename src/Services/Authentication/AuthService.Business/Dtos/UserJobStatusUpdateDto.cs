using AuthService.Core.Enums;
using FluentValidation;

namespace AuthService.Business.Dtos;

public record UserJobStatusUpdateDto
{
    public JobStatus JobStatus { get; set; }
}

public class UserJobStatusUpdateDtoValidator : AbstractValidator<UserJobStatusUpdateDto>
{
    public UserJobStatusUpdateDtoValidator()
    {
        RuleFor(x => x.JobStatus)
            .IsInEnum()
            .WithMessage("Düzgün iş statusu seçin");
    }
}
