using MassTransit;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.Company
{
    public class CompanyInformationService(IRequestClient<GetAllCompaniesRequest> _client, IRequestClient<GetCompanyDetailByIdRequest> getDetailClient) : ICompanyInformationService
    {
        public async Task<GetAllCompaniesResponse> GetCompaniesDataAsync()
        {
            var response = await _client.GetResponse<GetAllCompaniesResponse>(new GetAllCompaniesRequest());
            return response.Message;
        }

        public async Task<GetCompanyDetailByIdResponse> GetCompanyDetailByIdAsync(string companyId)
        {
            var guidCompanyId = Guid.Parse(companyId);

            var request = new GetCompanyDetailByIdRequest { CompanyId = guidCompanyId };

            var response = await getDetailClient.GetResponse<GetCompanyDetailByIdResponse>(request);

            return response.Message;
        }
    }
}