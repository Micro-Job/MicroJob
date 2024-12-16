using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.LanguageDtos
{
    public record LanguageUpdateListDto
    {
        public ICollection<LanguageUpdateDto> Languages { get; set; }
    }
}