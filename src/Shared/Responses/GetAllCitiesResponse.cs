using SharedLibrary.Dtos.CityDtos;

namespace SharedLibrary.Responses
{
    public class GetAllCitiesResponse
    {
        public int TotalCount { get; set; }
        public ICollection<CityDto> Cities { get; set; }
    }
}