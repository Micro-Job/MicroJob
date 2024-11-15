namespace JobCompany.Business.Services.CompanyServices
{
    public interface ICompanyService
    {
        Task UpdateCompanyAsync(Dtos.CompanyDtos.CompanyUpdateDto dto);
    }
}