using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class GetExamQuestionMappingConsumer(JobCompanyDbContext _dbContext) : IConsumer<GetExamQuestionMappingRequest>
{
    public async Task Consume(ConsumeContext<GetExamQuestionMappingRequest> context)
    {
        var examId = context.Message.ExamId;

        var examQuestions = await _dbContext.ExamQuestions
            .Where(eq => eq.ExamId == examId)
            .ToDictionaryAsync(eq => eq.QuestionId, eq => eq.Id);

        await context.RespondAsync(new GetExamQuestionMappingResponse
        {
            ExamQuestionMapping = examQuestions
        });
    }
}