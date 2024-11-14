using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.NumberDtos
{
    public record NumberCreateListDto
    {
        public ICollection<NumberCreateDto> PhoneNumbers { get; set; }
    }
}