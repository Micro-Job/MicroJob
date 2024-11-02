using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class Number : BaseEntity
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
        public string PhoneNumber { get; set; }
    }
}
