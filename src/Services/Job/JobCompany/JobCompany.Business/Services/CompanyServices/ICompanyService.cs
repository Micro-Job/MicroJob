using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Dtos.NumberDtos;
using Shared.Responses;

namespace JobCompany.Business.Services.CompanyServices
{
    public interface ICompanyService
    {
        Task UpdateCompanyAsync(CompanyUpdateDto dto, ICollection<UpdateNumberDto>? numbersDto);
        Task<DataListDto<CompanyDto>> GetAllCompaniesAsync(string? searchTerm, int skip = 1,int take = 12);
        //Task<GetAllCompaniesDataResponse> GetAllCompaniesDataResponseAsync(Guid UserId);
        Task<CompanyDetailItemDto> GetCompanyDetailAsync(string id);
        Task<CompanyProfileDto> GetOwnCompanyInformationAsync();
        Task<string> GetCompanyNameAsync(string id);
    }
}
