using SharedLibrary.Enums;

namespace Job.Core.Entities;

public class SkillTranslation : BaseEntity
{
    public string Name { get; set; }

    public LanguageCode Language { get; set; }

    public Guid SkillId { get; set; }
    public Skill Skill { get; set; }
}
