using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class SavedResume : BaseEntity
    {
        public Guid CompanyUserId { get; set; }
        //public User? CompanyUser { get; set; }

        public Guid ResumeId { get; set; }
        public Resume? Resume { get; set; }

        public DateTime SaveDate { get; set; }
    }
}
