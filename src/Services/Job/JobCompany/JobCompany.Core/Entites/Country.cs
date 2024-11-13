using JobCompany.Core.Entites.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Core.Entites
{
    internal class Country : BaseEntity
    {
        public string CountryName { get; set; }
        public ICollection<City> Cities { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
    }
}
