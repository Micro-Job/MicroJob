﻿using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class City : BaseEntity
    {
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
        public ICollection<Company> Companies { get; set; }

        public ICollection<CityTranslation> Translations { get; set; }

    }
}