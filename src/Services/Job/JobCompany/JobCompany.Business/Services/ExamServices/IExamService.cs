using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using Shared.Responses;

namespace JobCompany.Business.Services.ExamServices
{
    public interface IExamService
    {
        Task<Guid> CreateExamAsync(CreateExamDto dto);
        Task<List<ExamListDto>> GetExamsAsync(int skip, int take);
        Task<GetExamByIdDto> GetExamByIdAsync(string examId);
        Task<GetQuestionByStepDto> GetExamQuestionByStepAsync(string examId, int step);
        Task DeleteExamAsync(string examId);

        Task<GetExamDetailResponse> GetExamIntroAsync(string examId);
        Task<GetExamQuestionsDetailDto> GetExamQuestionsAsync(string examId);
        Task<SubmitExamResultDto> EvaluateExamAnswersAsync(SubmitExamAnswersDto dto);
    }
}