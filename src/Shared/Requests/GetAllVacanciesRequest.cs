using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary.Enums;

namespace Shared.Requests
{
    public class GetAllVacanciesRequest
    {
        public string? TitleName { get; set; }
        public string? CategoryId { get; set; }
        public string? CountryId { get; set; }
        public string? CityId { get; set; }
        public bool? IsActive { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
    }
}