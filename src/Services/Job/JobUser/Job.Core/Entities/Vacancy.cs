using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class Vacancy : BaseEntity
    {
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyLocation { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CompanyPhoto { get; set; }
        public decimal? EstimatedSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public int ViewCount { get; set; }
        public bool IsVip { get; set; }
        public WorkType WorkType { get; set; }
    }
}
