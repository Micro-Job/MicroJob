using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary.Enums;

namespace Shared.Dtos.ApplicationDtos
{
    public class ApplicationDetailDto
    {
        public string VacancyName { get; set; }
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public WorkType WorkType { get; set; }
        public WorkStyle WorkStyle { get; set; }
        public DateTime startDate { get; set; }
        public string ApplicationStatus { get; set; }
    }
}
