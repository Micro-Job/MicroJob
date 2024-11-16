using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.StatusDtos
{
    public record StatusListDto
    {
        public Guid StatusId { get; set; }
        public string StatusName { get; set; }
    }
}
