using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Core.Entites;

namespace JobCompany.Business.Services.QuestionServices
{
    public interface IQuestionService
    {
        Task CreateQuestionAsync(QuestionCreateDto dto, CreateListAnswersDto dtos);
        Task<ICollection<Question>> CreateBulkQuestionAsync(ICollection<QuestionCreateDto> dtos);
        Task<ICollection<Guid>> UpdateBulkQuestionsAsync(Guid examId, ICollection<QuestionUpdateDto> dtos);
    }
}