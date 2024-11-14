using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class CompanyNumber : BaseEntity
    {
        public string? Number { get; set; }
        public Guid? VacancyId { get; set; }
        public Vacancy? Vacancy { get; set; }
        public Guid? CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}