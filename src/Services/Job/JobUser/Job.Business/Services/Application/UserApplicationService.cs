using System.Security.Claims;
using Job.Business.Exceptions.ApplicationExceptions;
using Job.Business.Exceptions.Common;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Shared.Dtos.ApplicationDtos;
using Shared.Events;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.Application
{
    public class UserApplicationService : IUserApplicationService
    {
        readonly IPublishEndpoint _publishEndpoint;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestClient<GetUserApplicationsRequest> _userApplicationRequest;
        private readonly IRequestClient<CheckVacancyRequest> _requestClient;
        private readonly IRequestClient<GetUserDataRequest> _requestUser;
        private readonly IRequestClient<GetExamDetailRequest> _examRequest;
        private readonly IRequestClient<GetApplicationDetailRequest> _requestApplicationDetail;
        private readonly Guid userGuid;

        public UserApplicationService(
            IPublishEndpoint publishEndpoint,
            IHttpContextAccessor httpContextAccessor,
            IRequestClient<GetUserApplicationsRequest> userApplicationRequest,
            IRequestClient<CheckVacancyRequest> requestClient,
            IRequestClient<GetUserDataRequest> requestUser,
            IRequestClient<GetApplicationDetailRequest> requestApplicationDetail,
            IRequestClient<GetExamDetailRequest> examRequest
        )
        {
            _publishEndpoint = publishEndpoint;
            _httpContextAccessor = httpContextAccessor;
            _requestClient = requestClient;
            _userApplicationRequest = userApplicationRequest;
            userGuid = Guid.Parse(
                _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value
            );
            _requestUser = requestUser;
            _requestApplicationDetail = requestApplicationDetail;
            _examRequest = examRequest;
        }

        /// <summary> İstifadəçinin bütün müraciətlərini gətirir </summary>
        public async Task<ICollection<ApplicationDto>> GetUserApplicationsAsync(int skip, int take)
        {
            GetUserApplicationsRequest request = new()
            {
                UserId = userGuid,
                Skip = skip,
                Take = take,
            };

            var response = await _userApplicationRequest.GetResponse<GetUserApplicationsResponse>(
                request
            );

            return response.Message.UserApplications;
        }

        /// <summary> Eventle userin application yaratmasi  ve
        /// vakansiyaya muraciet ederken companye bildiris getmesi </summary>
        public async Task CreateUserApplicationAsync(string vacancyId)
        {
            var guidVac = Guid.Parse(vacancyId);

            var responseUser = await _requestUser.GetResponse<GetUserDataResponse>(
                new GetUserDataRequest { UserId = userGuid }
            );

            var response = await _requestClient.GetResponse<CheckVacancyResponse>(
                new CheckVacancyRequest { VacancyId = guidVac, UserId = userGuid }
            );

            if (!response.Message.IsExist)
                throw new EntityNotFoundException("Vacancy");
            var companyId = response.Message.CompanyId;

            if (response.Message.IsUserApplied)
                throw new ApplicationIsAlreadyExistException();

            await _publishEndpoint.Publish(
                new UserApplicationEvent
                {
                    UserId = userGuid,
                    VacancyId = guidVac,
                    CreatedDate = DateTime.Now,
                }
            );

            await _publishEndpoint.Publish(
                new VacancyApplicationEvent
                {
                    UserId = companyId,
                    SenderId = userGuid,
                    VacancyId = guidVac,
                    InformationId = userGuid,
                    Content =
                        $"İstifadəçi {responseUser.Message.FirstName} {responseUser.Message.LastName} {response.Message.VacancyName} vakansiyasına müraciət etdi.",
                }
            );
        }

        public async Task<GetApplicationDetailResponse> GetUserApplicationByIdAsync(
            string applicationId
        )
        {
            var response =
                await _requestApplicationDetail.GetResponse<GetApplicationDetailResponse>(
                    new GetApplicationDetailRequest { ApplicationId = applicationId }
                );
            return response.Message;
        }

        public async Task<GetExamDetailResponse> GetExamIntroAsync(string vacancyId)
        {
            var request = new GetExamDetailRequest { VacancyId = vacancyId };

            var response = await _examRequest.GetResponse<GetExamDetailResponse>(request);

            var userDataResponse = await _requestUser.GetResponse<GetUserDataResponse>(
                new GetUserDataRequest { UserId = userGuid }
            );

            var fullName =
                $"{userDataResponse.Message.FirstName} {userDataResponse.Message.LastName}";

            response.Message.FullName = fullName;

            return response.Message;
        }
    }
}
