using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites;

public class Message : BaseEntity
{
    public DateTime CreatedDate { get; set; }

    public ICollection<MessageTranslation> Translations { get; set; }
    public ICollection<VacancyMessage>? VacancyMessages { get; set; }
}