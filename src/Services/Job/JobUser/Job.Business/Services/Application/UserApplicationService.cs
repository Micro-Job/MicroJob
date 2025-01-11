using System.Security.Claims;
using Job.Business.Dtos.AnswerDtos;
using Job.Business.Dtos.ExamDtos;
using Job.Business.Dtos.QuestionDtos;
using Job.Business.Exceptions.ApplicationExceptions;
using Job.Business.Exceptions.Common;
using Job.Business.Exceptions.UserExceptions;
using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.ApplicationDtos;
using Shared.Events;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Dtos.QuestionDtos;
using SharedLibrary.Enums;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.Application
{
    public class UserApplicationService : IUserApplicationService
    {
        readonly IPublishEndpoint _publishEndpoint;
        private readonly JobDbContext _jobDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestClient<GetUserApplicationsRequest> _userApplicationRequest;
        private readonly IRequestClient<CheckVacancyRequest> _requestClient;
        private readonly IRequestClient<GetUserDataRequest> _requestUser;
        private readonly IRequestClient<GetExamDetailRequest> _examRequest;
        private readonly IRequestClient<GetApplicationDetailRequest> _requestApplicationDetail;
        private readonly IRequestClient<GetExamQuestionsRequest> _getExamQuestionsRequest;
        private readonly Guid userGuid;

        public UserApplicationService(
            JobDbContext jobDbContext,
            IPublishEndpoint publishEndpoint,
            IHttpContextAccessor httpContextAccessor,
            IRequestClient<GetUserApplicationsRequest> userApplicationRequest,
            IRequestClient<CheckVacancyRequest> requestClient,
            IRequestClient<GetUserDataRequest> requestUser,
            IRequestClient<GetApplicationDetailRequest> requestApplicationDetail,
            IRequestClient<GetExamDetailRequest> examRequest,
            IRequestClient<GetExamQuestionsRequest> getExamQuestionsRequest
        )
        {
            _jobDbContext = jobDbContext;
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
            _getExamQuestionsRequest = getExamQuestionsRequest;
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

            bool isTaken = await CheckUserCompletedExam(Guid.Parse(response.Message.ExamId));

            var fullName =
                $"{userDataResponse.Message.FirstName} {userDataResponse.Message.LastName}";

            response.Message.FullName = fullName;
            response.Message.IsTaken = isTaken;

            return response.Message;
        }

        public async Task<GetExamDetailResponse> GetExamFinalDescriptionAsync(string vacancyId)
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

        public async Task<GetExamQuestionsDetailDto> GetExamQuestionsAsync(Guid examId)
        {
            //var userExam = await _jobDbContext
            //    .UserExams.AsNoTracking()
            //    .FirstOrDefaultAsync(ue => ue.ExamId == examId && ue.UserId == userGuid);

            //if (userExam != null)
            //    throw new UserAlreadyCompletedExamException(
            //        "The user has already completed this exam."
            //    );

            var response = await FetchExamQuestionsAsync(examId);

            return new GetExamQuestionsDetailDto
            {
                LimitRate = response.LimitRate,
                TotalQuestions = response.Questions.Count,
                Duration = response.Duration,
                Questions = response.Questions.Select(q => new QuestionPublicDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    QuestionType = q.QuestionType,
                    Image = q.Image,
                    IsRequired = q.IsRequired,
                    Answers = q.Answers.Select(a => new AnswerPublicDto
                    {
                        Id = a.Id,
                        Text = a.Text,
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<SubmitExamResultDto> EvaluateExamAnswersAsync(SubmitExamAnswersDto dto)
        {
            var examQuestionsResponse = await FetchExamQuestionsAsync(dto.ExamId);
            var examQuestionMapping = await FetchExamQuestionMappingAsync(dto.ExamId);

            var questionDictionary = examQuestionsResponse.Questions.ToDictionary(
                q => q.Id,
                q => q
            );

            byte trueAnswerCount = 0;
            byte falseAnswerCount = 0;

            var userAnswers = dto
                .Answers.Select(userAnswer =>
                {
                    if (!questionDictionary.TryGetValue(userAnswer.QuestionId, out var question))
                        throw new EntityNotFoundException("Question");

                    if (!examQuestionMapping.TryGetValue(userAnswer.QuestionId, out var examQuestionId))
                        throw new EntityNotFoundException("Exam Question");

                    bool isCorrect = question.QuestionType switch
                    {
                        QuestionType.OpenEnded => true,

                        QuestionType.SingleChoice => question.Answers.Any(a =>
                            a.Id == userAnswer.AnswerIds?.FirstOrDefault() && a.IsCorrect == true
                        ),

                        QuestionType.MultipleChoice => ValidateMultipleChoice(question, userAnswer),

                        _ => false,
                    };

                    if (isCorrect)
                        trueAnswerCount++;
                    else
                        falseAnswerCount++;

                    return new UserAnswer
                    {
                        UserId = userGuid,
                        ExamQuestionId = examQuestionId,
                        Text = userAnswer.Text,
                        IsCorrect = isCorrect,
                    };
                })
                .ToList();

            await _jobDbContext.UserAnswers.AddRangeAsync(userAnswers);

            var userExam = new UserExam
            {
                UserId = userGuid,
                ExamId = dto.ExamId,
                TrueAnswerCount = trueAnswerCount,
                FalseAnswerCount = falseAnswerCount,
            };

            await _jobDbContext.UserExams.AddAsync(userExam);
            await _jobDbContext.SaveChangesAsync();

            decimal resultRate = trueAnswerCount * 100 / examQuestionsResponse.Questions.Count;

            bool isPassed = resultRate >= examQuestionsResponse.LimitRate;

            return new SubmitExamResultDto
            {
                TrueAnswerCount = trueAnswerCount,
                FalseAnswerCount = falseAnswerCount,
                ResultRate = resultRate,
                IsPassed = isPassed,
            };
        }

        private async Task<GetExamQuestionsResponse> FetchExamQuestionsAsync(Guid examId)
        {
            var request = new GetExamQuestionsRequest { ExamId = examId };

            var response = await _getExamQuestionsRequest.GetResponse<GetExamQuestionsResponse>(request);

            return response.Message;
        }

        private async Task<Dictionary<Guid, Guid>> FetchExamQuestionMappingAsync(Guid examId)
        {
            var request = new GetExamQuestionMappingRequest { ExamId = examId };

            var response = await _examRequest.GetResponse<GetExamQuestionMappingResponse>(request);

            return response.Message.ExamQuestionMapping;
        }

        private static bool ValidateMultipleChoice(
            QuestionDetailDto question,
            UserAnswerDto userAnswer
        )
        {
            var correctAnswers = question
                .Answers.Where(a => a.IsCorrect == true)
                .Select(a => a.Id)
                .ToList();

            var userSelectedAnswers =
                userAnswer.AnswerIds?.Where(id => id != Guid.Empty).ToList() ?? [];

            return correctAnswers.Count == userSelectedAnswers.Count
                && correctAnswers.All(userSelectedAnswers.Contains);
        }

        private async Task<bool> CheckUserCompletedExam(Guid examId)
        {
            var userExam = await _jobDbContext
                .UserExams.AsNoTracking()
                .FirstOrDefaultAsync(ue => ue.ExamId == examId && ue.UserId == userGuid);

            return userExam != null;
        }
    }
}
