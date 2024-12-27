using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class UserExam : BaseEntity
    {
        public Guid ExamId { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public byte TrueAnswerCount { get; set; }
        public byte FalseAnswerCount { get; set; }
    }
}