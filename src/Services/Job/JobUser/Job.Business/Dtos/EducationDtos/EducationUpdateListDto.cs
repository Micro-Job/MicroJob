using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.EducationDtos
{
    public class EducationUpdateListDto
    {
        public ICollection<EducationUpdateDto> Educations { get; set; }
    }
}