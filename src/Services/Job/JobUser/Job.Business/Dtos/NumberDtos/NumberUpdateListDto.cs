using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.NumberDtos
{
    public class NumberUpdateListDto
    {
        public ICollection<NumberUpdateDto> PhoneNumbers { get; set; }
    }
}