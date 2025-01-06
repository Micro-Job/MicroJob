using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Dtos.NumberDtos;
using Shared.Responses;

namespace JobCompany.Business.Services.CompanyServices
{
    public interface ICompanyService
    {
        Task UpdateCompanyAsync(CompanyUpdateDto dto, ICollection<UpdateNumberDto>? numbersDto);
        Task<ICollection<CompanyListDto>> GetAllCompaniesAsync(
            string? searchTerm,
            int skip = 1,
            int take = 12
        );
        Task<GetAllCompaniesDataResponse> GetAllCompaniesDataResponseAsync(Guid UserId);
        Task<CompanyDetailItemDto> GetCompanyDetailAsync(string id);
    }
}
