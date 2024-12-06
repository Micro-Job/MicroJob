using JobCompany.Business.Dtos.ExamDtos.AnswerDtos;

namespace JobCompany.Business.Services.ExamServices.AnswerServices
{
    public interface IAnswerService
    {
        Task CreateAnswerAsync(CreateAnswerDto dto);
    }
}