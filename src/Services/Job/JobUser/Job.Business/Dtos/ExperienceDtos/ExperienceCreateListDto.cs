using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.ExperienceDtos
{
    public record ExperienceCreateListDto
    {
        public ICollection<ExperienceCreateDto> Experiences { get; set; }
    }
}