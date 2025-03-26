using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.StatusDtos
{
    public class ChangeStatusOrderDto
    {
        public Guid StatusId { get; set; }
        public byte Order { get; set; }
    }
}
