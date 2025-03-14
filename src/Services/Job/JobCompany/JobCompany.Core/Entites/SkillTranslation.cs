using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites
{
    public class SkillTranslation : BaseEntity
    {
        public string Name { get; set; }

        public LanguageCode Language { get; set; }

        public Guid SkillId { get; set; }
        public Skill Skill { get; set; }

    }
}