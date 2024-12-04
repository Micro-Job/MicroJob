using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.CountryDtos
{
    public record CountryListDto
    {
        public Guid Id { get; set; }
        public string CountryName { get; set; }
    }
}