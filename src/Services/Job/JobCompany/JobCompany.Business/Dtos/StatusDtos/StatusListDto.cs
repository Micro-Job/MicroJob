using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.StatusDtos
{
    public record StatusListDto
    {
        public Guid StatusId { get; set; }
        public StatusEnum Status { get; set; }
        public string StatusColor { get; set; }
        public byte Order { get; set; }
        public bool IsVisible { get; set; }
    }
}