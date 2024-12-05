using JobCompany.Business.Dtos.ApplicationDtos;

namespace JobCompany.Business.Dtos.StatusDtos
{
    public record StatusListDtoWithApps
    {
        public Guid StatusId { get; set; }
        public string StatusName { get; set; }
        public string? StatusColor { get; set; }
        public bool IsDefault { get; set; }
        public List<ApplicationListDto> Applications { get; set; } = [];
    }
}