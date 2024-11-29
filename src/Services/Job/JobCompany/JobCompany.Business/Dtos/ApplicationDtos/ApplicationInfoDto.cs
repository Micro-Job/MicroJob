using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public class ApplicationInfoDto
    {
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}