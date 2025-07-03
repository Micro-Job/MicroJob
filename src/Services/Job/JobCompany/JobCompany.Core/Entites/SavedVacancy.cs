using JobCompany.Core.Entites.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Core.Entites
{
    public class SavedVacancy : BaseEntity
    {
        public Guid? UserId { get; set; }

        public Guid VacancyId { get; set; }
        public Vacancy? Vacancy { get; set; }

        public DateTime SavedAt { get; set; }
    }
}
