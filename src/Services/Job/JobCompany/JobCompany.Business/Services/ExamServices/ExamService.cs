using System.Security.Claims;
using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Business.Services.QuestionServices;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.ExamServices
{
    public class ExamService(
        JobCompanyDbContext _context,
        IQuestionService _questionService,
        IHttpContextAccessor _contextAccessor
    ) : IExamService
    {
        private readonly Guid userGuid = Guid.Parse(
            _contextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.Sid)?.Value!
        );

        public async Task<Guid> CreateExamAsync(CreateExamDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var company =
                await _context.Companies.FirstOrDefaultAsync(a => a.UserId == userGuid)
                ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>();

            try
            {
                var exam = new Exam
                {
                    Title = dto.Title,
                    IntroDescription = dto.IntroDescription,
                    LastDescription = dto.LastDescription,
                    Result = dto.Result,
                    LimitRate = dto.LimitRate,
                    CompanyId = company.Id,
                    IsTemplate = dto.IsTemplate,
                };

                await _context.Exams.AddAsync(exam);

                await _context.SaveChangesAsync();

                var examId = exam.Id.ToString();

                var questions = await _questionService.CreateBulkQuestionAsync(
                    dto.Questions,
                    examId
                );

                foreach (var question in questions)
                {
                    var examQuestion = new ExamQuestion
                    {
                        ExamId = exam.Id,
                        QuestionId = question.Id,
                    };

                    await _context.ExamQuestions.AddAsync(examQuestion);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return exam.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<GetExamByIdDto> GetExamByIdAsync(string examId)
        {
            var examGuid = Guid.Parse(examId);

            return await _context
                    .Exams.AsNoTracking()
                    .Select(e => new GetExamByIdDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        IntroDescription = e.IntroDescription,
                        LastDescription = e.LastDescription,
                        Duration = e.Duration,
                    })
                    .FirstOrDefaultAsync(e => e.Id == examGuid)
                ?? throw new SharedLibrary.Exceptions.NotFoundException<Exam>();
        }

        public async Task<GetQuestionByStepDto> GetExamQuestionByStepAsync(string examId, int step)
        {
            var examGuid = Guid.Parse(examId);

            var question =
                await _context
                    .Exams.Where(e => e.Id == examGuid)
                    .SelectMany(e => e.ExamQuestions)
                    .Skip(step - 1)
                    .Select(eq => new GetQuestionByStepDto
                    {
                        CurrentStep = step,
                        TotalSteps = eq.Exam.ExamQuestions.Count,
                        Question = new QuestionDetailDto
                        {
                            Id = eq.Question.Id,
                            Title = eq.Question.Title,
                            Image = eq.Question.Image,
                            QuestionType = eq.Question.QuestionType,
                            IsRequired = eq.Question.IsRequired,
                            Answers = eq
                                .Question.Answers.Select(a => new AnswerDetailDto
                                {
                                    Id = a.Id,
                                    Text = a.Text,
                                })
                                .ToList(),
                        },
                    })
                    .FirstOrDefaultAsync()
                ?? throw new SharedLibrary.Exceptions.NotFoundException<Question>();

            return question;
        }

        public async Task DeleteExamAsync(string examId)
        {
            var examGuid = Guid.Parse(examId);

            var exam =
                await _context.Exams.FirstOrDefaultAsync(e =>
                    e.Id == examGuid && e.Company.UserId == userGuid
                ) ?? throw new SharedLibrary.Exceptions.NotFoundException<Exam>();

            _context.Exams.Remove(exam);

            await _context.SaveChangesAsync();
        }

        public Task<List<ExamListDto>> GetExamsAsync(int skip, int take)
        {
            var exams = _context
                .Exams.Where(e => e.IsTemplate == true)
                .Select(e => new ExamListDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    QuestionCount = e.ExamQuestions.Count,
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return exams;
        }
    }
}
