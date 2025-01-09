using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Responses
{
    public class GetExamDetailResponse
    {
        public string ExamId { get; set; }
        public string CompanyName { get; set; }
        public int QuestionCount { get; set; }
        public byte? Duration { get; set; } 
        public decimal LimitRate { get; set; }
        public string FullName { get; set; }
        public string IntroDescription { get; set; }
    }
}
