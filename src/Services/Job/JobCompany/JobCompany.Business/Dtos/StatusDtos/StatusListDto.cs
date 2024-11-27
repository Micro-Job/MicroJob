namespace JobCompany.Business.Dtos.StatusDtos
{
    public record StatusListDto
    {
        public Guid StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
    }
}