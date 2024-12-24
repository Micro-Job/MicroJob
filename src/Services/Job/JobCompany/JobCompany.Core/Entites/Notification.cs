using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Notification : BaseEntity
    {
        public Guid ReceiverId { get; set; }
        public Company Receiver { get; set; }
        public Guid SenderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Content { get; set; }
        public bool IsSeen { get; set; }
        public Guid InformationId { get; set; }
    }
}