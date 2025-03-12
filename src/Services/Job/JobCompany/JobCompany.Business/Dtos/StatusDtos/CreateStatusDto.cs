using FluentValidation;
using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.StatusDtos
{
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
                    .WithMessage("Status adı boş ola bilməz!")
                .Length(2, 32)
                    .WithMessage("Adın uzunluğu 2-32 arasında olmalıdır.");
        }
    }
}