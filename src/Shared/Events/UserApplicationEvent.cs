using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class UserApplicationEvent
    {
        public Guid UserId { get; set; }
        public Guid VacancyId { get; set; }
    }
}