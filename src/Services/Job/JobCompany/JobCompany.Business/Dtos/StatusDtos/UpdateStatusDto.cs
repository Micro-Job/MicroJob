using FluentValidation;
using SharedLibrary.Enums;

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
                .WithMessage("Status adı boş ola bilməz!")
            .Length(2, 32)
                .WithMessage("Adın uzunluğu 2-32 arasında olmalıdır.");
    }
}
