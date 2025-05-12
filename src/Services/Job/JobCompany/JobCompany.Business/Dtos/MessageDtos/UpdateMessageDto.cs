using FluentValidation;

namespace JobCompany.Business.Dtos.MessageDtos;

public class UpdateMessageDto
{
    public List<MessageTranslationDto> Translations { get; set; }
}

public class UpdateMessageDtoValidator : AbstractValidator<UpdateMessageDto>
{
    public UpdateMessageDtoValidator()
    {
        RuleForEach(x => x.Translations)
            .SetValidator(new MessageTranslationDtoValidator());
    }
}