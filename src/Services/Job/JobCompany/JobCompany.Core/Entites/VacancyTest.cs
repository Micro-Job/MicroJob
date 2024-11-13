using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class VacancyTest : BaseEntity
    {
        public ICollection<Vacancy> Vacancies { get; set; }
    }
}