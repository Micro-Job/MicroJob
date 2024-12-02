using Shared.Responses;
using SharedLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Business.Services.Company
{
    public interface ICompanyInformationService
    {
        Task<GetAllCompaniesResponse> GetCompaniesDataAsync();
    }
}
