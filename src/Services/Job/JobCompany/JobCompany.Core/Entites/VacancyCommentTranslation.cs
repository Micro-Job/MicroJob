using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites;

public class VacancyCommentTranslation : BaseEntity
{
    public string Comment { get; set; }
    public LanguageCode Language { get; set; }
    public Guid VacancyCommentId { get; set; }
    public VacancyComment VacancyComment { get; set; }
}
