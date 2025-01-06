using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary.Enums;

namespace Shared.Responses
{
    public class GetApplicationDetailResponse
    {
        public string VacancyName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string Location { get; set; }
        public WorkType? WorkType { get; set; }
        public WorkStyle? WorkStyle { get; set; }
        public DateTime startDate { get; set; }
        public ICollection<string> CompanyStatuses { get; set; }
        public string ApplicationStatus { get; set; }
    }
}
