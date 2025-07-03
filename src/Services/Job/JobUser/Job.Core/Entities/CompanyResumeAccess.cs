using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class CompanyResumeAccess : BaseEntity
    {
        public Guid CompanyUserId { get; set; }
        //public User CompanyUser { get; set; }

        public Guid ResumeId { get; set; }
        public Resume Resume { get; set; }

        public DateTime? AccessDate { get; set; }
    }
}
