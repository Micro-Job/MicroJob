using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.ExamDtos
{
    public record ExamListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int QuestionCount { get; set; }
    }
}
