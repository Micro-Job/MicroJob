using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class Resume : BaseEntity
    {
        public Person Person { get; set; }
        public Guid PersonId { get; set; }
        public ICollection<Education> Educations { get; set; }
        public ICollection<Experience>? Experiences { get; set; }
        public ICollection<ExtraInformation>? ExtraInformations { get; set; }
    }
}
