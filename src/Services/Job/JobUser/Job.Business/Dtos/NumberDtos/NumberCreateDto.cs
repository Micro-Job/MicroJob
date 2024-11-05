using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.NumberDtos
{
    public record NumberCreateDto
    {
        public string ResumeId { get; set; }
        public string PhoneNumber { get; set; }
    }
}