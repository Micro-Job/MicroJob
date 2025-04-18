using JobCompany.Business.Dtos.NumberDtos;

namespace JobCompany.Business.Dtos.CompanyDtos
{
    public record UpdateCompanyRequest
    {
        public CompanyUpdateDto? CompanyDto { get; set; }
        public ICollection<UpdateNumberDto>? NumbersDto { get; set; }
    }
}