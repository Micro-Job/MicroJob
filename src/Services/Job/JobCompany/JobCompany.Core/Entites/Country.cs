using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Country : BaseEntity
    {
        public string CountryName { get; set; }
        public ICollection<City> Cities { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
        public ICollection<Company> Companies { get; set; }
    }
}