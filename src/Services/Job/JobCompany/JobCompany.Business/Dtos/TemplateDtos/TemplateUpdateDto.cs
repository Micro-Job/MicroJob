using FluentValidation;

namespace JobCompany.Business.Dtos.TemplateDtos;

public record TemplateUpdateDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class TemplateUpdateDtoValidator : AbstractValidator<TemplateUpdateDto>
{
    public TemplateUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Şablon id boş ola bilməz");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Şablon adı boş ola bilməz");
        RuleFor(x => x.Name).MaximumLength(50).WithMessage("Şablon adı maksimum 50 simvoldan ibarət ola bilər");
    }
}
