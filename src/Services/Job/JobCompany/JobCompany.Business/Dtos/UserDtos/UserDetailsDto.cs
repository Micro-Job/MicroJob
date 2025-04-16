using JobCompany.Business.Dtos.Common;
using Shared.Responses;
using SharedLibrary.Responses;

namespace JobCompany.Business.Dtos.UserDtos;

public class UserDetailsDto
{
    public GetResumeDataResponse? PersonalInformation { get; set; }
    public GetResumeDetailResponse? ResumeDetails { get; set; }
    public DataListDto<GetUserTransactionsResponse>? UserTransactions { get; set; }
}
