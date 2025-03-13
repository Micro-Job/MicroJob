using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.CategoryDtos;

public class CategoryGetByIdDto
{
    public Guid Id { get; set; }
    public List<CategoryTranslationGetByIdDto> CategoryTranslations { get; set; }
}
public record CategoryTranslationGetByIdDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public LanguageCode LanguageCode { get; set; }
}
