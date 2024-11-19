using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public record ApplicationListDto
    {
        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }
        public Guid VacancyId { get; set; }
        public bool IsActive { get; set; }
    }
}
