using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.MessageDtos;

public class MessageTranslationDto
{
    public string Content { get; set; }
    public LanguageCode Language { get; set; }
}

