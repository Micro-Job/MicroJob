using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;

namespace Job.Business.Dtos.ResumeDtos
{
    public record ResumeUpdateListDto
    {
        public NumberUpdateListDto NumberUpdateDtos { get; set; }
        public ExperienceUpdateListDto ExperienceUpdateDtos { get; set; }
        public EducationUpdateListDto EducationUpdateDtos { get; set; }
        public LanguageUpdateListDto LanguageUpdateDtos { get; set; }
    }
}