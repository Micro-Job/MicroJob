using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class VacancyNumber : BaseEntity
    {
        public string? Number { get; set; }
        public Guid? VacancyId { get; set; }
        public Vacancy? Vacancy { get; set; }
    }
}