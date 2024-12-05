namespace JobCompany.Business.Dtos.CountryDtos
{
    public record CountryListDto
    {
        public Guid Id { get; set; }
        public string CountryName { get; set; }
    }
}