using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.CompanyDtos
{
    public class CompanyMiniDto
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
