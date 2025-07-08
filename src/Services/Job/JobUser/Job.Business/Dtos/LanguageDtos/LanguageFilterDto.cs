using Job.Core.Enums;

namespace Job.Business.Dtos.LanguageDtos;

public class LanguageFilterDto
{
    public Language Language { get; set; }
    public LanguageLevel LanguageLevel { get; set; }
}
