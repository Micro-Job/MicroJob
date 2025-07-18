using FluentValidation;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.MessageDtos;

public class MessageTranslationDto
{
    public string Content { get; set; }
    public LanguageCode Language { get; set; }
}

public class MessageTranslationDtoValidator : AbstractValidator<MessageTranslationDto>
{
    public MessageTranslationDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(MessageHelper.GetMessage("CANNOT_BE_EMPTY"))
            .MaximumLength(500).WithMessage(x =>
                MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.Content?.Length ?? 0, 500));
    }
}
