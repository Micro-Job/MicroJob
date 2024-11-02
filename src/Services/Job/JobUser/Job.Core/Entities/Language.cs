using Job.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class Language : BaseEntity
    {
        public ExtraInformation ExtraInformation { get; set; }
        public Guid ExtraInformationId { get; set; }
        public Enums.Language LanguageName { get; set; }
        public LanguageLevel LanguageLevel { get; set; }
    }
}
