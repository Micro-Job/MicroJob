using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.LanguageDtos
{
    public class LanguageCreateListDto
    {
        public ICollection<LanguageCreateDto> Languages { get; set; }
    }
}