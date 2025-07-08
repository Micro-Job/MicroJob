using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public class ChangeApplicationStatusDto
    {
        public Guid ApplicationId { get; set; }
        public Guid StatusId { get; set; }
        public string Message { get; set; }
    }
}
