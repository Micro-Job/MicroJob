using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.ExamDtos
{
    public class UserExamDetailDto
    {
        public float TotalPercent { get; set; }

        public byte TrueAnswerCount { get; set; }
        public byte FalseAnswerCount { get; set; }
    }
}
