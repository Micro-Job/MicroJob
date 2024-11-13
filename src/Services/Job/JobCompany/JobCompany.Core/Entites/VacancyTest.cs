using JobCompany.Core.Entites.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Core.Entites
{
    internal class VacancyTest : BaseEntity
    {
        public ICollection<Vacancy> Vacancies { get; set; }
    }
}
