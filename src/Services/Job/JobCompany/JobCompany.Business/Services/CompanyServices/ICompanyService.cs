using JobCompany.Business.Dtos.CompanyDtos;
using Shared.Responses;

namespace JobCompany.Business.Services.CompanyServices
{
    public interface ICompanyService
    {
        Task UpdateCompanyAsync(CompanyUpdateDto dto);
        Task<ICollection<CompanyListDto>> GetAllCompanies(int skip = 1, int take = 12);
        Task<GetAllCompaniesDataResponse> GetAllCompaniesDataResponseAsync (Guid UserId);
        Task<CompanyDetailItemDto> GetCompanyDetailAsync(string id);
    }
}