using FluentValidation;

namespace JobCompany.Business.Dtos.TemplateDtos;

public record TemplateCreateDto
{
    public string Title { get; set; }
}

public class TemplateCreateDtoValidator : AbstractValidator<TemplateCreateDto>
{
    public TemplateCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Şablon adı boş ola bilməz")
                             .MaximumLength(50).WithMessage("Şablon adı maksimum 50 simvoldan ibarət ola bilər");
    }
}