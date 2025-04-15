namespace JobCompany.Business.Dtos.MessageDtos;

public class MessageWithTranslationsDto
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<MessageTranslationDto> Translations { get; set; }
}

