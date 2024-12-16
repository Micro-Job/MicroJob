using FluentValidation;

namespace JobCompany.Business.Dtos.StatusDtos
{
    public record CreateStatusDto
    {
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
        public byte Order { get; set; }
    }

    public class CreateStatusValidator : AbstractValidator<CreateStatusDto>
    {
        public CreateStatusValidator()
        {
            RuleFor(x => x.StatusName.Trim())
                .NotEmpty()
                    .WithMessage("Status adı boş ola bilməz!")
                .Length(2, 32)
                    .WithMessage("Adın uzunluğu 2-32 arasında olmalıdır.");
        }
    }
}