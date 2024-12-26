namespace JobCompany.Business.Dtos.NumberDtos
{
    public record CompanyNumberDto
    {
        public Guid Id { get; set; }
        public string? Number { get; set; }
    }
}