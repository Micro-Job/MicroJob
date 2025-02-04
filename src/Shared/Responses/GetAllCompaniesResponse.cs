using SharedLibrary.Dtos.CompanyDtos;

namespace SharedLibrary.Responses
{
    public class GetAllCompaniesResponse
    {
        public ICollection<CompanyDto> Companies { get; set; }
        public int TotalCount { get; set; }
    }
}
