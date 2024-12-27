namespace Job.Business.Dtos.NotificationDtos;

public record NotificationListDto
{
    public Guid Id { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid SenderId { get; set; }
    public Guid InformationId { get; set; }
    public string CompanyName { get; set; }
    public string CompanyLogo { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Content { get; set; }
    public bool IsSeen { get; set; }
}
