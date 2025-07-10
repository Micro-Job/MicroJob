using Job.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class ResumeLink : BaseEntity
    {
        public LinkEnum LinkType { get; set; }
        public string Url { get; set; }

        public Guid ResumeId { get; set; }
        public Resume Resume { get; set; }
    }
}
