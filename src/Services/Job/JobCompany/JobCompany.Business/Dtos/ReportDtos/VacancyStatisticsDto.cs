using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.ReportDtos
{
    public record VacancyStatisticsDto
    {
        public int TotalVacancies { get; set; }
        public PercentageChangeDto PercentageChange { get; set; }
        public List<MonthlyStatisticDto> MonthlyStatistics { get; set; }
        public List<ApplicationDetailDto> Applications { get; set; }
    }
}