using FluentValidation;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.SkillDtos;

public class SkillCreateDto
{
    public List<CreateSkillLanguageDto> Skills { get; set; }
}

public class CreateSkillLanguageDto
{
    public string Name { get; set; }
    public LanguageCode Language { get; set; }
}


public class CreateSkillLanguageDtoValidator : AbstractValidator<CreateSkillLanguageDto>
{
    public CreateSkillLanguageDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
        RuleFor(x => x.Language).NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
    }
}
