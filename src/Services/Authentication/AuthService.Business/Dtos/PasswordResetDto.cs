using FluentValidation;
using SharedLibrary.Helpers;
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
        }
    }
}
