using SharedLibrary.Responses;

namespace Job.Business.Services.City
{
    public interface ICityService
    {
        Task<GetAllCitiesResponse> GetAllCitiesAsync(Guid countryId, int skip = 1, int take = 6);
        Task<GetAllCountriesResponse> GetAllCountriesAsync(int skip = 1, int take = 6);
    }
}