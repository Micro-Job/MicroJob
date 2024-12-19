using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.CompanyDtos
{
    public record CompanyItemDto
    {
        public string CompanyName { get; set; }
        public string? CompanyImage { get; set; }
    }
}