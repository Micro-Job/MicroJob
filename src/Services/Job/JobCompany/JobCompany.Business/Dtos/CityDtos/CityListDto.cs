namespace JobCompany.Business.Dtos.CityDtos
{
    public record CityListDto
    {
        public string CityName { get; set; }
        public Guid CountryId { get; set; }
    }
}