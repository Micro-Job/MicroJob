using FluentValidation;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

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
            .WithMessage(MessageHelper.GetMessage("NOT_FOUND"));
    }
}
