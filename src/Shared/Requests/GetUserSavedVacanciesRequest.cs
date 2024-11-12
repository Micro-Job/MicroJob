using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Requests
{
    public class GetUserSavedVacanciesRequest
    {
        public List<Guid> VacancyIds { get; set; }
    }
}
