using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Requests
{
    public class GetAllVacanciesByCompanyIdDataRequest
    {
        public Guid CompanyId { get; set; } 
    }
}