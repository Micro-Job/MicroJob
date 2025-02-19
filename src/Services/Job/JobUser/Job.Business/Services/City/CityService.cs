using MassTransit;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.City
{
    public class CityService(IRequestClient<GetAllCitiesRequest> requestClient, IRequestClient<GetAllCountriesRequest> counttryRequestClient) : ICityService
    {
        private readonly IRequestClient<GetAllCitiesRequest> _requestClient = requestClient;
        private readonly IRequestClient<GetAllCountriesRequest> _countryRequestClient = counttryRequestClient;

        public async Task<GetAllCitiesResponse> GetAllCitiesAsync(Guid countryId, int skip = 1, int take = 6)
        {
            var response = await _requestClient.GetResponse<GetAllCitiesResponse>(new GetAllCitiesRequest
            {
                Skip = skip,
                Take = take,
                CountryId = countryId
            });

            return new GetAllCitiesResponse
            {
                Cities = response.Message.Cities,
                TotalCount = response.Message.TotalCount,
            };
        }

        public async Task<GetAllCountriesResponse> GetAllCountriesAsync(int skip = 1, int take = 6)
        {
            var response = await _countryRequestClient.GetResponse<GetAllCountriesResponse>(new GetAllCountriesRequest
            {
                Skip = skip,
                Take = take,
            });

            return new GetAllCountriesResponse
            {
                Countries = response.Message.Countries,
                TotalCount = response.Message.TotalCount,
            };
        }
    }
}