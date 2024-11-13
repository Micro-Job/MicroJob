using JobCompany.Core.Entites.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Core.Entites
{
    internal class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
    }
}
