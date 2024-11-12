using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Responses
{
    public class GetUserSavedVacanciesResponse
    {
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyLocation { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CompanyPhoto { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public int ViewCount { get; set; }
        public bool IsVip { get; set; }
        public WorkType WorkType { get; set; }
    }
}
