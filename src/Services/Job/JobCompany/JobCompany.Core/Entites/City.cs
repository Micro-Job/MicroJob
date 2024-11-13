using JobCompany.Core.Entites.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Core.Entites
{
    internal class City : BaseEntity
    {
        public string CityName { get; set; }
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }

    }
}
