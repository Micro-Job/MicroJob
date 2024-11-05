using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.NumberDtos
{
    public class NumberDetailItemDto
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public string PhoneNumber { get; set; }
    }
}