using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites;

//TODO : VacancyComment heç bir yerdə istifadə edilmir. Lazimliligi arasdirilib, silinmelidir
public class VacancyComment : BaseEntity
{
    public CommentType CommentType { get; set; }
    public ICollection<VacancyCommentTranslation> Translations { get; set; }
    public ICollection<Vacancy> Vacancies { get; set; }
}
