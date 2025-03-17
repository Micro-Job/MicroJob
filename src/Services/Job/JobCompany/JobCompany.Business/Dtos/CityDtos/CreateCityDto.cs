using FluentValidation;
using JobCompany.Business.Dtos.CategoryDtos;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.CityDtos;

public class CreateCityDto
{
    public Guid CountryId { get; set; }
    public List<CreateCityLanguageDto> Cities { get; set; }
}

public class CreateCityLanguageDto
{
    public string Name { get; set; }
    public LanguageCode language { get; set; }
}


public class CreateCityLanguageDtoValidator : AbstractValidator<CreateCityLanguageDto>
{
    public CreateCityLanguageDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
        RuleFor(x => x.language).NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
    }
}


public class CreateCityDtoValidator : AbstractValidator<CreateCityDto>
{
    public CreateCityDtoValidator()
    {
        RuleForEach(x => x.Cities).SetValidator(new CreateCityLanguageDtoValidator());
        RuleFor(x => x.Cities)
            .Must(skills => skills.Select(s => s.language).Distinct().Count() >= 3)
            .WithMessage(MessageHelper.GetMessage("ALL_LANGUAGES_REQUIRED"));
    }
}