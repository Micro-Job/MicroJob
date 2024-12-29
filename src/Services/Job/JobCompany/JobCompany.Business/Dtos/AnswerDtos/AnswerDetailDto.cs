using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.AnswerDtos
{
    public record AnswerDetailDto
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public bool? IsCorrect { get; set; }
    }
}