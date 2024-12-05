namespace JobCompany.Business.Dtos.CityDtos
{
    public record CityListDto
    {
        public Guid Id { get; set; }
        public string CityName { get; set; }
        public Guid CountryId { get; set; }
    }
}