using AuthService.Business.Dtos;

namespace AuthService.Business.Services.CompanyServices
{
    public interface ICompanyService
    {
        Task UpdateCompanyAsync(CompanyUpdateDto dto);
    }
}