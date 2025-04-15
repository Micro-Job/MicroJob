using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites;

public class MessageTranslation : BaseEntity
{
    public string Content { get; set; }
    public LanguageCode Language { get; set; }

    public Guid MessageId { get; set; }
    public Message Message { get; set; }
}
