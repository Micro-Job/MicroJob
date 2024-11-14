using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;

namespace Job.Business.Dtos.ResumeDtos
{
    public class ResumeCreateListsDto
    {
        public NumberCreateListDto NumberCreateDtos { get; set; }
        public ExperienceCreateListDto ExperienceCreateDtos { get; set; }
        public EducationCreateListDto EducationCreateDtos { get; set; }
        public LanguageCreateListDto LanguageCreateDtos { get; set; }
    }
}