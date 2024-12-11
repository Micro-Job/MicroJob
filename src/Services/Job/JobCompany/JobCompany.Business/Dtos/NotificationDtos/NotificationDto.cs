namespace JobCompany.Business.Dtos.NotificationDtos
{
    public class NotificationDto
    {
        public Guid ReceiverId { get; set; }
        public Guid SenderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Content { get; set; }
    }

}