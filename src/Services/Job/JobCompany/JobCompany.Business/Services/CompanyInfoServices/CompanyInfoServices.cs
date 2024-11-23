using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Shared.Requests;
using Shared.Responses;

namespace JobCompany.Business.Services.CompanyInfoServices
{
    public class CompanyInfoServices(IRequestClient<GetAllCompaniesDataRequest> _client) : ICompanyInfoServices
    {
        public async Task<GetAllCompaniesDataResponse> GetAllCompaniesDataResponseAsync (Guid UserId)
        {
            var response = await _client.GetResponse<GetAllCompaniesDataResponse>(new GetAllCompaniesDataRequest { UserId = UserId });
            return response.Message;
        }
    }
}