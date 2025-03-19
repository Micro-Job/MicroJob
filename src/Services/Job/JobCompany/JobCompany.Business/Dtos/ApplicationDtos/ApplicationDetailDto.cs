using JobCompany.Business.Dtos.StatusDtos;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public class ApplicationDetailDto
    {
        public Guid VacancyId { get; set; }
        public string? VacancyName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyLogo { get; set; }
        public string? Location { get; set; }
        public WorkType? WorkType { get; set; }
        public WorkStyle? WorkStyle { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ApplicationStatusName { get; set; }
        public Guid? ApplicationStatusId { get; set; }
        public ICollection<ApplicationStatusesListDto>? CompanyStatuses { get; set; }
    }
}
