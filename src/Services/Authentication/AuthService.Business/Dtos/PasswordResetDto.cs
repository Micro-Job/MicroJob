using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Dtos
{
    public class PasswordResetDto
    {
        public string Email { get; set; }

        public string Token { get; set; }

        public string NewPassword { get; set; }
    }

  
    public class PasswordResetDtoValidator : AbstractValidator<PasswordResetDto>
    {
        public PasswordResetDtoValidator()
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
