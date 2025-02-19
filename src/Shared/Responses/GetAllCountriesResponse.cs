using SharedLibrary.Dtos.CountryDtos;

namespace SharedLibrary.Responses
{
    public class GetAllCountriesResponse
    {
        public int TotalCount { get; set; }
        public ICollection<CountryDto> Countries { get; set; }
    }
}