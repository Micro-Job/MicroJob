using SharedLibrary.Responses;

namespace Job.Business.Services.Company
{
    public interface ICompanyInformationService
    {
        Task<GetAllCompaniesResponse> GetCompaniesDataAsync();
        Task<GetCompanyDetailByIdResponse> GetCompanyDetailByIdAsync(string companyId);
    }
}
