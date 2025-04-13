using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Core.Entites;

namespace JobCompany.Business.Services.AnswerServices
{
    public interface IAnswerService
    {
        Task<ICollection<Answer>> CreateBulkAnswerAsync(
            ICollection<CreateAnswerDto> dtos,
            string questionId
        );

        Task UpdateBulkAnswersAsync(List<Question> existingQuestions, ICollection<QuestionUpdateDto> dtos);
    }
}
