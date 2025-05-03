using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites;

public class Notification : BaseEntity
{
    public Guid ReceiverId { get; set; }
    public Company Receiver { get; set; }
    public Guid? SenderId { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsSeen { get; set; }
    public Guid InformationId { get; set; }
    public string? InformationName { get; set; }
    public string? SenderName {  get; set; }
    public string? SenderImage { get; set; }
    public NotificationType NotificationType { get; set; }
}