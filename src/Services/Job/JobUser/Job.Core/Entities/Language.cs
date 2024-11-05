using Job.Core.Enums;

namespace Job.Core.Entities
{
    public class Language : BaseEntity
    {
        public Resume Resume { get; set; }
        public Guid ResumeId { get; set; }
        public Enums.Language LanguageName { get; set; }
        public LanguageLevel LanguageLevel { get; set; }
    }
}