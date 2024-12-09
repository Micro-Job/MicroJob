using JobCompany.Business.Dtos.ExamDtos;

namespace JobCompany.Business.Services.ExamServices
{
    public interface IExamServices
    {
        Task CreateExamAsync(CreateExamDto dto);
        Task<GetExamByIdDto> GetExamByIdAsync(string examId, byte step);
    }
}