using FluentValidation;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace AuthService.Business.Dtos;

public class OperatorAddDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MainPhoneNumber { get; set; }
    public UserRole UserRole { get; set; }
}

public class OperatorAddDtoValidator : AbstractValidator<OperatorAddDto>
{
    public OperatorAddDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty().WithMessage(MessageHelper.GetMessage("CANNOT_BE_EMPTY"))
            .Length(1, 50).WithMessage(x => MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.FirstName?.Length ?? 0, 50));

        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty().WithMessage(MessageHelper.GetMessage("CANNOT_BE_EMPTY"))
            .Length(1, 50).WithMessage(x => MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.LastName?.Length ?? 0, 50));

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty().WithMessage(MessageHelper.GetMessage("CANNOT_BE_EMPTY"))
            .EmailAddress().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"))
            .MaximumLength(100).WithMessage(x => MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.Email?.Length ?? 0, 100));

        RuleFor(x => x.MainPhoneNumber)
            .NotNull()
            .NotEmpty().WithMessage(MessageHelper.GetMessage("CANNOT_BE_EMPTY"))
            .Matches(@"^(?:\+\d{1,3})?\d{1,4}\d{7,10}$")
            .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
    }
}