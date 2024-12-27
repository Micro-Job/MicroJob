using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class UserAnswer : BaseEntity
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string? Text { get; set; }
        public bool? IsCorrect { get; set; }
        public Guid ExamQuestionId { get; set; }
    }
}