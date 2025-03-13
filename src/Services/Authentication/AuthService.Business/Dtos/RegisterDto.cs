using FluentValidation;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Helpers;

namespace AuthService.Business.Dtos
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MainPhoneNumber { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public IFormFile? Image { get; set; }
        public byte UserStatus {  get; set; }
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
           
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
                .NotNull()
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("CANNOT_BE_EMPTY"))
                .MinimumLength(8)
                .WithMessage(MessageHelper.GetMessage("PASSWORD_MIN_LENGTH"))
                .Matches(@"[A-Z]")
                .WithMessage(MessageHelper.GetMessage("PASSWORD_MUST_CONTAIN_UPPERCASE"))
                .Matches(@"[a-z]")
                .WithMessage(MessageHelper.GetMessage("PASSWORD_MUST_CONTAIN_LOWERCASE"))
                .Matches(@"[0-9]")
                .WithMessage(MessageHelper.GetMessage("PASSWORD_MUST_CONTAIN_NUMBER"))
                .Matches(@"[\W_]")
                .WithMessage(MessageHelper.GetMessage("PASSWORD_MUST_CONTAIN_SPECIAL_CHAR"))
                .Length(8, 100)
                .WithMessage(MessageHelper.GetMessage("PASSWORD_LENGTH_RANGE"));
            //RuleFor(x => x.PhoneNumber)
            //    .Matches(@"^(?:\+\d{1,3})?\d{1,4}\d{7,10}$")
            //    .WithMessage("Telefon nömrəsi doğru formatda deyil");
        }
    }
}
