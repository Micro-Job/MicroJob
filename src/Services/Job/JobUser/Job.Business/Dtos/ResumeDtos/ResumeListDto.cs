using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Business.Dtos.ResumeDtos
{
    public class ResumeListDto
    {
        public Guid Id { get; set; }
        public bool IsSaved { get; set; }
        public string? ProfileImage { get; set; }
        public string? FullName { get; set; }
        public string? Position { get; set; }
        public JobStatus JobStatus { get; set; }
        public LastWorkDto? LastWork { get; set; }
        public ICollection<string>? SkillsName { get; set; }
    }
}
