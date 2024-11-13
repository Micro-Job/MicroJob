using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class City : BaseEntity
    {
        public string CityName { get; set; }
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }

    }
}