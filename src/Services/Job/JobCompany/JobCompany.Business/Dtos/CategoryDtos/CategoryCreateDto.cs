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


public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateDtoValidator()
    {
        RuleForEach(x => x.Categories).SetValidator(new CategoryCreateLanguageDtoValidator());
        RuleFor(x => x.Categories)
            .Must(skills => skills.Select(s => s.language).Distinct().Count() >= 3)
            .WithMessage(MessageHelper.GetMessage("ALL_LANGUAGES_REQUIRED"));
    }
}