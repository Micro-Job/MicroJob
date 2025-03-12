using FluentValidation;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.CategoryDtos;

public class CategoryCreateDto
{
    public List<CategoryCreateLanguageDto> Categories { get; set; }
}

public class CategoryCreateLanguageDto
{
    public string Name { get; set; }
    public LanguageCode language { get; set; }
}


public class CategoryCreateLanguageDtoValidator : AbstractValidator<CategoryCreateLanguageDto>
{
    public CategoryCreateLanguageDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
        RuleFor(x => x.language).NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
    }
}
