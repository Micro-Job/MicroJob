using SharedLibrary.Enums;

namespace Job.Core.Entities;

public class NotificationTranslation : BaseEntity
{
    public string? Content { get; set; }
    public LanguageCode Language { get; set; }
    public Notification Notification { get; set; }
    public Guid NotificationId { get; set; }
}