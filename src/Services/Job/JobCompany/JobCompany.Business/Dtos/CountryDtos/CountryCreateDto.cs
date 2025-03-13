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
