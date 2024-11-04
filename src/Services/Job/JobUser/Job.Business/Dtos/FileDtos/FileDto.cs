using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.FileDtos
{
    public record FileDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}