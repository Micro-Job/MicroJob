using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.ReportDtos
{
    public class MonthlyStatisticDto
    {
        public string Month { get; set; }
        public int Value { get; set; }
        public bool IsHighlighted { get; set; }
    }
}