using SharedLibrary.Enums;
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
        public byte Order { get; set; }
        public StatusEnum CompanyStatus { get; set; }
    }
}
