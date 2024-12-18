namespace JobCompany.Business.Dtos.CompanyDtos
{
    public record CompanyInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Information {  get; set; }
    }
}