using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Core.Enums;

namespace JobCompany.Business.Dtos.QuestionDtos
{
    public record QuestionDetailDto
    {
        public string Title { get; set; }
        public string? Image { get; set; }
        public QuestionType QuestionType { get; set; }
        public bool IsRequired { get; set; }
        public ICollection<AnswerDetailDto> Answers { get; set; }
    }
}