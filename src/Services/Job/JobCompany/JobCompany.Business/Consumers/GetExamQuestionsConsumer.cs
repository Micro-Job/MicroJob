using JobCompany.Business.Exceptions.Common;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.AnswerDtos;
using SharedLibrary.Dtos.QuestionDtos;
using SharedLibrary.Enums;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class GetExamQuestionsConsumer(JobCompanyDbContext _dbContext) : IConsumer<GetExamQuestionsRequest>
{
    public async Task Consume(ConsumeContext<GetExamQuestionsRequest> context)
    {
        var examData = await _dbContext.Exams
        .Where(e => e.Id == context.Message.ExamId)
        .Select(e => new
        {
            e.LimitRate,
            Questions = e.ExamQuestions.Select(eq => new QuestionDetailDto
            {
                Id = eq.Question.Id,
                Title = eq.Question.Title,
                QuestionType = (QuestionType)eq.Question.QuestionType,
                Image = eq.Question.Image ?? string.Empty,
                IsRequired = eq.Question.IsRequired,
                Answers = eq.Question.Answers.Select(a => new AnswerDetailDto
                {
                    Id = a.Id,
                    Text = a.Text,
                    IsCorrect = a.IsCorrect
                }).ToList()
            }).ToList()
        })
        .FirstOrDefaultAsync()
        ?? throw new NotFoundException<Exam>();

        await context.RespondAsync(new GetExamQuestionsResponse
        {
            Questions = examData.Questions,
            LimitRate = examData.LimitRate
        });
    }
}
