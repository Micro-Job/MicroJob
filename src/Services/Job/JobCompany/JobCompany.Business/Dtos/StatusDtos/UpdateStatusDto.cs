using FluentValidation;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.StatusDtos;

public class UpdateStatusDto
{
    public Guid Id { get; set; }
    public string StatusColor { get; set; }
    public byte Order { get; set; }
    public List<UpdateStatusLanguageDto> Statuses { get; set; }
}

public class UpdateStatusLanguageDto
{
    public string Name { get; set; }
    public LanguageCode Language { get; set; }
}


public class UpdateStatusLanguageDtoValidator : AbstractValidator<UpdateStatusLanguageDto>
{
    public UpdateStatusLanguageDtoValidator()
    {
        RuleFor(x => x.Name.Trim())
            .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
            .Length(2, 32)
                .WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_50"));
    }
}
