using FluentValidation;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.VacancyComment;

public class VacancyCommentCreateDto
{
    public CommentType CommentType { get; set; }
    public List<VacancyCommentCreateLanguageDto> VacancyComments { get; set; }
}
public class VacancyCommentCreateLanguageDto
{
    public string Comment { get; set; }
    public LanguageCode language { get; set; }
}
public class VacancyCommentCreateLanguageDtoValidator : AbstractValidator<VacancyCommentCreateLanguageDto>
{
    public VacancyCommentCreateLanguageDtoValidator()
    {
        RuleFor(x => x.Comment).NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
        RuleFor(x => x.language).NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));
    }
}

public class VacancyCommentCreateDtoValidator : AbstractValidator<VacancyCommentCreateDto>
{
    public VacancyCommentCreateDtoValidator()
    {
        RuleForEach(x => x.VacancyComments).SetValidator(new VacancyCommentCreateLanguageDtoValidator());
        RuleFor(x => x.VacancyComments)
            .Must(skills => skills.Select(s => s.language).Distinct().Count() >= 3)
            .WithMessage(MessageHelper.GetMessage("ALL_LANGUAGES_REQUIRED"));
    }
}