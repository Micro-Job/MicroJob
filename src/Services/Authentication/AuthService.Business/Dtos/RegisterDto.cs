using FluentValidation;

namespace AuthService.Business.Dtos
{
    public class RegisterDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Boş ola bilməz")
                .Length(1, 32)
                .WithMessage("Uzunluq 1-50 arasında olmalıdır");
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
            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage("Boş ola bilməz")
                .MinimumLength(8)
                .WithMessage("Şifrə ən az 8 simvol uzunluğunda olmalıdır")
                .Matches(@"[A-Z]")
                .WithMessage("Şifrədə ən az bir böyük hərf olmalıdır")
                .Matches(@"[a-z]")
                .WithMessage("Şifrədə ən az bir kiçik hərf olmalıdır")
                .Matches(@"[0-9]")
                .WithMessage("Şifrədə ən az bir rəqəm olmalıdır")
                .Matches(@"[\W_]")
                .WithMessage("Şifrədə ən az bir xüsusi simvol olmalıdır")
                .Length(8, 100)
                .WithMessage("Uzunluq 8-100 arasında olmalıdır");
            //RuleFor(x => x.PhoneNumber)
            //    .Matches(@"^(?:\+\d{1,3})?\d{1,4}\d{7,10}$")
            //    .WithMessage("Telefon nömrəsi doğru formatda deyil");
        }
    }
}
