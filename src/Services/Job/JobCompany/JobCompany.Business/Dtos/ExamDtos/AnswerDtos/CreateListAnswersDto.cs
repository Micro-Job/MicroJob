using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.ExamDtos.AnswerDtos
{
    public class CreateListAnswersDto
    {
        public ICollection<CreateAnswerDto> Answers { get; set; }
    }
}