using FluentValidation;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.CountryDtos;

public class CountryCreateDto
{
    public List<CreateCountryLanguageDto> Countries { get; set; }
}

public class CreateCountryLanguageDto
{
    public string Name { get; set; }
    public LanguageCode Language { get; set; }
}


public class CreateCountryLanguageDtoValidator : AbstractValidator<CreateCountryLanguageDto>
{
    public CreateCountryLanguageDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
        RuleFor(x => x.Language).NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
    }
}


public class CountryCreateDtoValidator : AbstractValidator<CountryCreateDto>
{
    public CountryCreateDtoValidator()
    {
        RuleForEach(x => x.Countries).SetValidator(new CreateCountryLanguageDtoValidator());
        RuleFor(x => x.Countries)
            .Must(skills => skills.Select(s => s.Language).Distinct().Count() >= 3)
            .WithMessage(MessageHelper.GetMessage("ALL_LANGUAGES_REQUIRED"));
    }
}