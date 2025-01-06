using MassTransit;
using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.Company
{
    public class CompanyInformationService(
        IRequestClient<GetAllCompaniesRequest> _client,
        IRequestClient<GetCompanyDetailByIdRequest> getDetailClient
    ) : ICompanyInformationService
    {
        public async Task<ICollection<CompanyDto>> GetCompaniesDataAsync(
            string? searchTerm,
            int skip,
            int take
        )
        {
            var response = await _client.GetResponse<GetAllCompaniesResponse>(
                new GetAllCompaniesRequest
                {
                    SearchTerm = searchTerm,
                    Skip = skip,
                    Take = take,
                }
            );
            return response.Message.Companies;
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
