using JobCompany.Business.Dtos.CompanyDtos;

namespace JobCompany.Business.Services.CompanyServices
{
    public interface ICompanyService
    {
        Task UpdateCompanyAsync(CompanyUpdateDto dto);

        Task<ICollection<CompanyListDto>> GetAllCompanies();
        Task<CompanyDetailItemDto> GetCompanyDetailAsync(string id);
    }
}