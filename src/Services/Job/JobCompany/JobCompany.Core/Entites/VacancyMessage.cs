using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites;

public class VacancyMessage : BaseEntity
{
    public Guid VacancyId { get; set; }
    public Vacancy Vacancy { get; set; }
    public Guid MessageId { get; set; }
    public Message Message { get; set; }
}
