using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.VacancyComment;

public class VacancyCommentDetailDto
{
    public Guid Id { get; set; }
    public List<VacancyCommentTranslationDetailDto> VacancyCommentTranslations { get; set; }
}
public record VacancyCommentTranslationDetailDto
{
    public Guid Id { get; set; }
    public string Comment { get; set; }
    public LanguageCode LanguageCode { get; set; }
}
