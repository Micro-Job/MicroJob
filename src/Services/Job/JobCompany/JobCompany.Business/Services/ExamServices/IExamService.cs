using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using Shared.Responses;

namespace JobCompany.Business.Services.ExamServices
{
    public interface IExamService
    {
        Task<Guid> CreateExamAsync(CreateExamDto dto);
        Task UpdateExamAsync(UpdateExamDto dto);
        Task<DataListDto<ExamListDto>> GetExamsAsync(string? examName, int skip, int take);
        Task<GetExamByIdDto> GetExamByIdAsync(string examId);
        Task<GetQuestionByStepDto> GetExamQuestionByStepAsync(string examId, int step);
        Task DeleteExamAsync(string examId);

        Task<GetExamIntroDto> GetExamIntroAsync(string examId, string vacancyId);
        Task<GetExamQuestionsDetailDto> GetExamQuestionsAsync(string examId);
        Task<GetExamQuestionsDetailDto> GetExamQuestionsForUserAsync(string examId);
        Task<SubmitExamResultDto> EvaluateExamAnswersAsync(SubmitExamAnswersDto dto);
    }
}