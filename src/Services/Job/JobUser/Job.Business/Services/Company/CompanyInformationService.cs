using MassTransit;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
