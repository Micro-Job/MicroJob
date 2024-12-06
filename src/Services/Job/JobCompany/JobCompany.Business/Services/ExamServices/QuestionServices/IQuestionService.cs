using JobCompany.Business.Dtos.ExamDtos.QuestionDtos;
using JobCompany.Core.Entites;

namespace JobCompany.Business.Services.ExamServices.QuestionServices
{
    public interface IQuestionService
    {
        Task CreateQuestionAsync(QuestionCreateDto dto);
        Task<ICollection<Question>> CreateBulkQuestionAsync(ICollection<QuestionCreateDto> dtos, string examId);
    }
}