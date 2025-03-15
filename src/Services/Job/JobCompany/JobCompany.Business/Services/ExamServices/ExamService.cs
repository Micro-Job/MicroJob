using System.Security.Claims;
using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Business.Services.QuestionServices;
using JobCompany.Core.Entites;
using JobCompany.Core.Enums;
using JobCompany.DAL.Contexts;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Responses;
using SharedLibrary.Exceptions;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.ExamServices
{
    public class ExamService(
        JobCompanyDbContext _context,
        IQuestionService _questionService,
        IHttpContextAccessor _contextAccessor,
        ICurrentUser _user
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
                    Duration = dto.Duration,
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
                            Image = eq.Question.Image != null ? $"{_user.BaseUrl}/{eq.Question.Image}" : null,
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

        public async Task<GetExamDetailResponse> GetExamIntroAsync(string examId)
        {
            var examGuid = Guid.Parse(examId);
            var data = await _context.Exams.Where(x=> x.Id == examGuid)
                .Select(x=> new GetExamDetailResponse
                {
                    CompanyName = x.Company.CompanyName,
                    IntroDescription = x.IntroDescription,
                    Duration = x.Duration,
                    //IsTaken = x.UserExams.Any(ue => ue.ExamId == examId && ue.UserId == userGuid);
                    QuestionCount = x.ExamQuestions.Count,
                    LimitRate = x.LimitRate,
                })
                .FirstOrDefaultAsync() ?? throw new NotFoundException<Exam>("İmtahan mövcud deyil");

            return data;
        }

        public async Task<GetExamQuestionsDetailDto> GetExamQuestionsAsync(string examId)
        {
            var examGuid = Guid.Parse(examId);
            var exam = await _context.Exams
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.Question)
                .ThenInclude(q => q.Answers)
                .Where(x => x.Id == examGuid)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException<Exam>();

            return new GetExamQuestionsDetailDto
            {
                TotalQuestions = exam.ExamQuestions.Count,
                LimitRate = exam.LimitRate,
                Duration = exam.Duration,
                Questions = exam.ExamQuestions.Select(eq => new QuestionPublicDto
                {
                    Id = eq.Question.Id,
                    Title = eq.Question.Title,
                    Image = eq.Question.Image != null ? $"{_user.BaseUrl}/{eq.Question.Image}" : null,
                    QuestionType = eq.Question.QuestionType,
                    IsRequired = eq.Question.IsRequired,
                    Answers = eq.Question.Answers.Select(a => new AnswerPublicDto
                    {
                        Id = a.Id,
                        Text = a.Text
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<SubmitExamResultDto> EvaluateExamAnswersAsync(SubmitExamAnswersDto dto)
        {
            var userGuid = _user.UserGuid ?? throw new InvalidOperationException("UserId can not be null");
            var exam = await _context.Exams
                .Include(e => e.ExamQuestions)
                .ThenInclude(eq => eq.Question)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(x => x.Id == dto.ExamId)
                ?? throw new NotFoundException<Exam>();

            var answerResults = exam.ExamQuestions
                .Select(eq =>
                {
                    var question = eq.Question;
                    var userAnswer = dto.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                    if (userAnswer == null) return false;

                    var correctAnswers = question.Answers
                        .Where(a => a.IsCorrect ?? false)
                        .Select(a => a.Id)
                        .ToList();

                    return question.QuestionType switch
                    {
                        QuestionType.MultipleChoice or QuestionType.SingleChoice =>
                            userAnswer.AnswerIds?.Count == correctAnswers.Count &&
                            !userAnswer.AnswerIds.Except(correctAnswers).Any(),

                        QuestionType.OpenEnded =>
                            !string.IsNullOrEmpty(userAnswer.Text) &&
                            string.Equals(
                                userAnswer.Text.Trim(),
                                question.Answers.FirstOrDefault(a => a.IsCorrect ?? false)?.Text?.Trim(),
                                StringComparison.OrdinalIgnoreCase
                            ),

                        _ => false
                    };
                })
                .ToList();

            int trueCount = answerResults.Count(isCorrect => isCorrect);
            int falseCount = answerResults.Count(isCorrect => !isCorrect);

            int totalQuestions = exam.ExamQuestions.Count;
            decimal resultRate = totalQuestions > 0 ? (decimal)trueCount / totalQuestions * 100 : 0;
            bool isPassed = resultRate >= exam.LimitRate;

            var userExam = new UserExam
            {
                UserId = userGuid, 
                ExamId = dto.ExamId,
                TrueAnswerCount = (byte)trueCount,
                FalseAnswerCount = (byte)falseCount, 
            };


            _context.UserExams.Add(userExam);
            await _context.SaveChangesAsync(); 

            return new SubmitExamResultDto
            {
                TrueAnswerCount = (byte)trueCount,
                FalseAnswerCount = (byte)falseCount,
                ResultRate = resultRate,
                IsPassed = isPassed
            };
        }
    }
}
