using Job.Business.Dtos.ExamDtos;
using Job.Business.Dtos.QuestionDtos;
using Shared.Responses;
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Responses;

namespace Job.Business.Services.Application
{
    public interface IUserApplicationService
    {
        //Task<GetExamDetailResponse> GetExamIntroAsync(string vacancyId);
        Task<GetExamQuestionsDetailDto> GetExamQuestionsAsync(Guid examId);
        Task<SubmitExamResultDto> EvaluateExamAnswersAsync(SubmitExamAnswersDto dto);
    }
}
