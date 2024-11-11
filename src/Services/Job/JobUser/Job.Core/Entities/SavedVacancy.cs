using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class SavedVacancy : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid VacancyId { get; set; }
    }
}
