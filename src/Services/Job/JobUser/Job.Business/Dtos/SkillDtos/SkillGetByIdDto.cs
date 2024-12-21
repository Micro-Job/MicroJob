using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.SkillDtos
{
    public record SkillGetByIdDto
    {
        public string Skill { get; set; }
    }
}