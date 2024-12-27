using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class ExamQuestion : BaseEntity
    {
        public Exam Exam { get; set; }
        public Guid ExamId { get; set; }
        public Question Question { get; set; }
        public Guid QuestionId { get; set; }
    }
}