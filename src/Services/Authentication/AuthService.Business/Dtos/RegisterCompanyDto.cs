using FluentValidation;
using SharedLibrary.Helpers;

namespace AuthService.Business.Dtos;

public class RegisterCompanyDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; } 
    public string? CompanyName { get; set; }
    public string MainPhoneNumber { get; set; } 
    public string Email { get; set; } 
    public string Password { get; set; } 
    public string ConfirmPassword { get; set; } 
    public bool Policy { get; set; }
    public bool IsCompany { get; set; }
    public string? VOEN { get; set; }
}

public class RegisterCompanyValidator : AbstractValidator<RegisterCompanyDto>
{
    public RegisterCompanyValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .When(x=> x.IsCompany)
            .WithMessage("CANNOT_BE_EMPTY")
            .Length(1, 100)
            .When(x => x.IsCompany)
            .WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"));
        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty()
            .WithMessage(MessageHelper.GetMessage("CANNOT_BE_EMPTY"))
            .Length(1, 50)
            .WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_50"));
        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty()
            .WithMessage(MessageHelper.GetMessage("CANNOT_BE_EMPTY"))
            .Length(1, 50)
            .WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_50"));
        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .WithMessage(MessageHelper.GetMessage("CANNOT_BE_EMPTY"))
            .EmailAddress()
            .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(MessageHelper.GetMessage("CANNOT_BE_EMPTY"));
            //.MinimumLength(8)
            //.WithMessage(MessageHelper.GetMessage("PASSWORD_MIN_LENGTH"))
            //.Matches(@"[A-Z]")
            //.WithMessage(MessageHelper.GetMessage("PASSWORD_MUST_CONTAIN_UPPERCASE"))
            //.Matches(@"[a-z]")
            //.WithMessage(MessageHelper.GetMessage("PASSWORD_MUST_CONTAIN_LOWERCASE"))
            //.Matches(@"[0-9]")
            //.WithMessage(MessageHelper.GetMessage("PASSWORD_MUST_CONTAIN_NUMBER"))
            //.Matches(@"[\W_]")
            //.WithMessage(MessageHelper.GetMessage("PASSWORD_MUST_CONTAIN_SPECIAL_CHAR"))
            //.Length(8, 100)
            //.WithMessage(MessageHelper.GetMessage("PASSWORD_LENGTH_RANGE"));
        //write a rule that password and confirm password must be equal
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage(MessageHelper.GetMessage("CANNOT_BE_EMPTY"))
            .Equal(x => x.Password)
            .WithMessage(MessageHelper.GetMessage("PASSWORDS_DO_NOT_MATCH"));

        RuleFor(x => x.VOEN)
            .NotEmpty()
            .When(x => x.IsCompany)
            .WithMessage("CANNOT_BE_EMPTY");
    }
}
