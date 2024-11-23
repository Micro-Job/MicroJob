﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.CompanyDtos
{
    public record CompanyListDto
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyImage { get; set; }
        public int CompanyVacancyCount { get; set; }
    }
}