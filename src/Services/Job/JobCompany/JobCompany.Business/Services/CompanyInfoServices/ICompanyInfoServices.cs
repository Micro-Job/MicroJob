using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Responses;

namespace JobCompany.Business.Services.CompanyInfoServices
{
    public interface ICompanyInfoServices
    {
         Task<GetAllCompaniesDataResponse> GetAllCompaniesDataResponseAsync(Guid UserId);
    }
}