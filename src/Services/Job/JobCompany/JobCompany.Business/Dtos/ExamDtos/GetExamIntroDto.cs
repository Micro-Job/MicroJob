using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.ExamDtos
{
    public class GetExamIntroDto
    {
        public string CompanyName { get; set; }
        public int QuestionCount { get; set; }
        public short? Duration { get; set; }
        public decimal LimitRate { get; set; }
        public string FullName { get; set; }
        public string IntroDescription { get; set; }
    }
}
