namespace Job.Business.Dtos.NotificationDtos
{
    public record NotificationByUserDto
    {
        public Guid SenderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid InformationId { get; set; }
        public string? Content { get; set; }
        public bool IsSeen { get; set; }
    }
}