using JobCompany.Business.Dtos.ApplicationDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.StatusDtos
{
    public record StatusListDtoWithApps
    {
        public Guid StatusId { get; set; }
        public string StatusName { get; set; }
        public string? StatusColor { get; set; }
        public bool IsDefault { get; set; }
        public List<ApplicationListDto> Applications { get; set; } = new();
    }
}
