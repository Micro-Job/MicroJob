using JobCompany.Business.Dtos.ExamDtos.AnswerDtos;
using JobCompany.Core.Entites;

namespace JobCompany.Business.Services.AnswerServices
{
    public interface IAnswerService
    {
        Task<ICollection<Answer>> CreateBulkAnswerAsync(ICollection<CreateAnswerDto> dtos, string questionId);

    }
}