using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.NumberDtos
{
    public record NumberListDto
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public string PhoneNumber { get; set; }
    }
}