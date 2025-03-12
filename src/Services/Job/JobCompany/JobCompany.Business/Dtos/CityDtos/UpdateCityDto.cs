namespace JobCompany.Business.Dtos.CityDtos
{
    public record UpdateCityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}