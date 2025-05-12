using FluentValidation;

namespace JobCompany.Business.Dtos.MessageDtos;

public class CreateMessageDto
{
    public List<MessageTranslationDto> Translations { get; set; }
}

public class CreateMessageDtoValidator : AbstractValidator<CreateMessageDto>
{
    public CreateMessageDtoValidator()
    {
        RuleForEach(x => x.Translations)
            .SetValidator(new MessageTranslationDtoValidator());
    }
}

