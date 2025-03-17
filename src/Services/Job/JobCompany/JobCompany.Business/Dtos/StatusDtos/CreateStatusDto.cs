using FluentValidation;
using JobCompany.Business.Dtos.CategoryDtos;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.StatusDtos;

public record CreateStatusDto
{
    public string StatusColor { get; set; }
    public byte Order { get; set; }
    public List<CreateStatusLanguageDto> Statuses { get; set; }
}

public class CreateStatusLanguageDto
{
    public string Name { get; set; }
    public LanguageCode Language { get; set; }
}


public class CreateStatusLanguageDtoValidator : AbstractValidator<CreateStatusLanguageDto>
{
    public CreateStatusLanguageDtoValidator()
    {
        RuleFor(x => x.Name.Trim())
            .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
            .Length(2, 32)
                .WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_50"));
    }
}

public class CreateStatusDtoValidator : AbstractValidator<CreateStatusDto>
{
    public CreateStatusDtoValidator()
    {
        RuleForEach(x => x.Statuses).SetValidator(new CreateStatusLanguageDtoValidator());
        RuleFor(x => x.Statuses)
            .Must(skills => skills.Select(s => s.Language).Distinct().Count() >= 3)
            .WithMessage(MessageHelper.GetMessage("ALL_LANGUAGES_REQUIRED"));
    }
}