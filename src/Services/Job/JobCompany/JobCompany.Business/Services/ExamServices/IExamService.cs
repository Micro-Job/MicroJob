using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Dtos.QuestionDtos;

namespace JobCompany.Business.Services.ExamServices
{
    public interface IExamService
    {
        Task<Guid> CreateExamAsync(CreateExamDto dto);
        Task<GetExamByIdDto> GetExamByIdAsync(string examId);
        Task<GetQuestionByStepDto> GetExamQuestionByStepAsync(string examId, int step);
    }
}