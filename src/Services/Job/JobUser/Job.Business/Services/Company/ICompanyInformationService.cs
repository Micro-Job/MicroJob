using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Responses;

namespace Job.Business.Services.Company
{
    public interface ICompanyInformationService
    {
        Task<ICollection<CompanyDto>> GetCompaniesDataAsync(string? searchTerm);
        Task<GetCompanyDetailByIdResponse> GetCompanyDetailByIdAsync(string companyId);
    }
}