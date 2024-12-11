using MassTransit;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.Company
{
    public class CompanyInformationService(IRequestClient<GetAllCompaniesRequest> _client) : ICompanyInformationService
    {
        public async Task<GetAllCompaniesResponse> GetCompaniesDataAsync()
        {
            var response = await _client.GetResponse<GetAllCompaniesResponse>(new GetAllCompaniesRequest());
            return response.Message;
        }
    }
}