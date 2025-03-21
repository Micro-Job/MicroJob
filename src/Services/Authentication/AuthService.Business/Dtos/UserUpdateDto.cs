using FluentValidation;
using SharedLibrary.Helpers;

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

            RuleFor(x => x.MainPhoneNumber)
                .Matches(@"^(?:\+\d{1,3})?\d{1,4}\d{7,10}$")
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
        }
    }
}