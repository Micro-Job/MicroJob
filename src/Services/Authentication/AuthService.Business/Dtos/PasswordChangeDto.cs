using FluentValidation;

namespace AuthService.Business.Dtos
{
    public class PasswordChangeDto
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }

    public class PasswordChangeDtoValidator : AbstractValidator<PasswordChangeDto>
    {
        public PasswordChangeDtoValidator()
        {
            RuleFor(x => x.NewPassword)
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
        }
    }
}
