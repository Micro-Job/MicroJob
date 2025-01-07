using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;

namespace JobCompany.Business.Services.AnswerServices
{
    public class AnswerService(JobCompanyDbContext context) : IAnswerService
    {
        private readonly JobCompanyDbContext _context = context;

        public async Task<ICollection<Answer>> CreateBulkAnswerAsync(
            ICollection<CreateAnswerDto> dtos,
            string questionId
        )
        {
            Guid questionGuid = Guid.Parse(questionId);
            var answersToAdd = dtos.Select(dto => new Answer
                {
                    Text = dto.Text,
                    QuestionId = questionGuid,
                    IsCorrect = dto.IsCorrect,
                })
                .ToList();
            await _context.Answers.AddRangeAsync(answersToAdd);
            return answersToAdd;
        }
    }
}
