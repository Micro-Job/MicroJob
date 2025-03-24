using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Business.Dtos.ResumeDtos
{
    public class SavedResumeListDto
    {
        public Guid Id { get; set; }
        public Guid ResumeId { get; set; }
        public int MyProperty { get; set; }
    }
}
