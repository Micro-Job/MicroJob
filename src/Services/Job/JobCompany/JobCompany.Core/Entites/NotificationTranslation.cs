using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites;

public class NotificationTranslation : BaseEntity
{
    public string? Content { get; set; }
    public LanguageCode Language { get; set; }
    public Notification Notification { get; set; }
    public Guid NotificationId { get; set; }
}
