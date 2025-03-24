using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.StatusDtos
{
    public class ApplicationStatusesListDto
    {
        public Guid CompanyStatusId { get; set; }
        public int Order { get; set; }
        public string CompanyStatusName { get; set; }
    }
}
