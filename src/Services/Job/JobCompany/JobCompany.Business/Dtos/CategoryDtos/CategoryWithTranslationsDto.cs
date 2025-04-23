using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.CategoryDtos;

public class CategoryWithTranslationsDto
{
    public Guid Id { get; set; }
    public bool IsCompany { get; set; }
    public List<CategoryTranslationDto> Translations { get; set; }
}

public class CategoryTranslationDto
{
    public string Name { get; set; }
    public LanguageCode Language { get; set; }
}

