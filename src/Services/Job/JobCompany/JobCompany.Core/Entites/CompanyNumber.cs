using JobCompany.Core.Entites.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Core.Entites
{
    internal class CompanyNumber : BaseEntity
    {
        public string? Number { get; set; }
        public Guid VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }
    }
}
