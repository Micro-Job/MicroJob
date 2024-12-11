using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Dtos.CompanyDtos
{
    public class CompanyDetailDto
    {
        public Guid? CompanyId { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? CityId { get; set; }
    }
}