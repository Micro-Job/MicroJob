using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;

namespace JobCompany.Business.Services.AnswerServices
{
    public class AnswerService(JobCompanyDbContext _context) : IAnswerService
    {
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
            }).ToList();
            await _context.Answers.AddRangeAsync(answersToAdd);
            return answersToAdd;
        }

        public async Task UpdateBulkAnswersAsync(List<Question> existingQuestions, ICollection<QuestionUpdateDto> dtos)
        {
            var answersToAdd = new List<Answer>(); // yeni cavabları əlavə etmək üçün
            var answersToRemove = new List<Answer>(); // silinməsi lazım olan cavabları saxlamaq üçün

            var existingQuestionsDict = existingQuestions.ToDictionary(q => q.Id);

            foreach (var questionDto in dtos)
            {
                if (!questionDto.Id.HasValue) continue;

                if (!existingQuestionsDict.TryGetValue(questionDto.Id.Value, out var question))
                    continue;

                var existingAnswers = question.Answers ?? [];

                var existingAnswersDict = existingAnswers.ToDictionary(a => a.Id);

                var dtoAnswerIds = questionDto.Answers
                    .Where(a => a.Id.HasValue)
                    .Select(a => a.Id.Value)
                    .ToList();

                // Mövcud cavabları güncəlləyir
                foreach (var dtoAnswer in questionDto.Answers.Where(a => a.Id.HasValue))
                {
                    if (existingAnswersDict.TryGetValue(dtoAnswer.Id.Value, out var answer))
                    {
                        answer.Text = dtoAnswer.Text;
                        answer.IsCorrect = dtoAnswer.IsCorrect;
                    }
                }

                // Yeni cavabları əlavə edir
                foreach (var dtoAnswer in questionDto.Answers.Where(a => !a.Id.HasValue))
                {
                    var newAnswer = new Answer
                    {
                        QuestionId = question.Id,
                        Text = dtoAnswer.Text,
                        IsCorrect = dtoAnswer.IsCorrect
                    };
                    answersToAdd.Add(newAnswer);
                }

                // Id-ləri olmayan cavabları silir
                var removedAnswers = existingAnswers
                    .Where(a => !dtoAnswerIds.Contains(a.Id))
                    .ToList();

                answersToRemove.AddRange(removedAnswers);
            }

            if (answersToRemove.Count != 0)
                _context.Answers.RemoveRange(answersToRemove);

            if (answersToAdd.Count != 0)
                await _context.Answers.AddRangeAsync(answersToAdd);
        }
    }
}