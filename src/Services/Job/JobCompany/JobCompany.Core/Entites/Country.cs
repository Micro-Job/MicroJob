using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Country : BaseEntity
    {
        public ICollection<City> Cities { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
        public ICollection<Company> Companies { get; set; }

        public ICollection<CountryTranslation> Translations { get; set; }

    }

}