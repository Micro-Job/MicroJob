using AuthService.Core.Enums;
using FluentValidation;

namespace AuthService.Business.Dtos
{
    public class UserUpdateDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MainPhoneNumber { get; set; }
    }

    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                 .NotNull()
                 .NotEmpty()
                 .WithMessage("Boş ola bilməz")
                 .Length(1, 32)
                 .WithMessage("Uzunluq 1-50 arasında olmalıdır");

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Boş ola bilməz")
                .Length(1, 32)
                .WithMessage("Uzunluq 1-50 arasında olmalıdır");

            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .WithMessage("Boş ola bilməz")
                .EmailAddress()
                .WithMessage("E-mail doğru formatda deyil");

            RuleFor(x => x.MainPhoneNumber)
                .Matches(@"^(?:\+\d{1,3})?\d{1,4}\d{7,10}$")
                .WithMessage("Telefon nömrəsi doğru formatda deyil");
        }
    }
}