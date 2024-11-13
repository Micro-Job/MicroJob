using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
    }
}