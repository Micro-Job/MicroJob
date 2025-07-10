using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Business.Services.QuestionServices;
using JobCompany.Core.Entites;
using JobCompany.Core.Enums;
using JobCompany.DAL.Contexts;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using System.Threading;

namespace JobCompany.Business.Services.ExamServices;

public class ExamService(JobCompanyDbContext _context, QuestionService _questionService, ICurrentUser _currentUser)
{
    public async Task<Guid> CreateExamAsync(CreateExamDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        var companyId = await _context.Companies
            .Where(a => a.UserId == _currentUser.UserGuid)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        try
        {
            var exam = new Exam
            {
                Title = dto.Title!,
                IntroDescription = dto.IntroDescription!,
                LimitRate = dto.LimitRate,
                CompanyId = companyId,
                IsTemplate = dto.IsTemplate,
                Duration = dto.Duration,
                CreatedDate = DateTime.Now
            };

            await _context.Exams.AddAsync(exam);
            await _context.SaveChangesAsync();

            var questions = await _questionService.CreateBulkQuestionAsync(dto.Questions);

            var examQuestions = questions.Select(x => new ExamQuestion
            {
                ExamId = exam.Id,
                QuestionId = x.Id
            }).ToList();

            await _context.ExamQuestions.AddRangeAsync(examQuestions);
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

    public async Task UpdateExamAsync(UpdateExamDto dto)
    {
        var exam = await _context.Exams
            .Include(e => e.ExamQuestions)
            .ThenInclude(eq => eq.Question)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(e => e.Id == dto.Id && e.Company.UserId == _currentUser.UserGuid)
            ?? throw new NotFoundException();

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            exam.Title = dto.Title!;
            exam.IntroDescription = dto.IntroDescription!;
            exam.LimitRate = dto.LimitRate;
            exam.IsTemplate = dto.IsTemplate;
            exam.Duration = dto.Duration;

            var updatedQuestionIds = await _questionService.UpdateBulkQuestionsAsync(dto.Id, dto.Questions!);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<GetExamByIdDto> GetExamByIdAsync(Guid examId)
    {
        return await _context.Exams.AsNoTracking()
                .Select(e => new GetExamByIdDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    IntroDescription = e.IntroDescription,
                    Duration = e.Duration,
                    LimitRate = e.LimitRate,
                    IsTemplate = e.IsTemplate
                })
                .FirstOrDefaultAsync(e => e.Id == examId)
            ?? throw new NotFoundException();
    }

    public async Task<GetQuestionByStepDto> GetExamQuestionByStepAsync(Guid examId, int step)
    {
        var question = await _context.Exams
            .AsNoTracking()
            .Where(e => e.Id == examId)
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
                    Image = eq.Question.Image != null ? $"{_currentUser.BaseUrl}/companyFiles/{eq.Question.Image}" : null,
                    QuestionType = eq.Question.QuestionType,
                    IsRequired = eq.Question.IsRequired,
                    Order = eq.Question.Order,
                    Answers = eq.Question.Answers!.Select(a => new AnswerDetailDto
                    {
                        Id = a.Id,
                        Text = a.Text,
                    }).ToList(),
                }
            }).FirstOrDefaultAsync()
            ?? throw new NotFoundException();

        return question;
    }

    public async Task DeleteExamAsync(Guid examId)
    {
        var affectedRows = await _context.Exams
            .Where(e =>e.Id == examId && e.Company.UserId == _currentUser.UserGuid)
            .ExecuteDeleteAsync();

        if (affectedRows == 0)
            throw new NotFoundException();
    }

    public async Task<DataListDto<ExamListDto>> GetExamsAsync(string? examName, int skip, int take)
    {
        var query = _context.Exams
            .Where(e => e.IsTemplate == true && e.Company.UserId == _currentUser.UserGuid)
            .OrderByDescending(x => x.CreatedDate)
            .AsNoTracking();

        if (examName != null)
            query = query.Where(x => x.Title.Contains(examName));

        var exams = await query
            .Select(e => new ExamListDto
            {
                Id = e.Id,
                Title = e.Title,
                QuestionCount = e.ExamQuestions.Count,
            })
            .Skip(Math.Max(0, (skip - 1) * take))
            .Take(take)
            .ToListAsync();

        return new DataListDto<ExamListDto>
        {
            Datas = exams,
            TotalCount = await query.CountAsync(),
        };
    }

    public async Task<GetExamIntroDto> GetExamIntroAsync(Guid examId, Guid vacancyId)
    {
        var hasTakenExam = await _context.UserExams.AnyAsync(x => x.ExamId == examId && x.UserId == _currentUser.UserGuid && x.VacancyId == vacancyId);

        if (hasTakenExam)
            return new GetExamIntroDto { IsTaken = true };

        var data = await _context.Exams.Where(x => x.Id == examId)
            .AsNoTracking()
            .Select(x => new GetExamIntroDto
            {
                //TODO : burada isaxtaran eger imtahan yaratsa bu zaman Company.CompanyName bu zaman vacancy.CompanyName olmalidir
                CompanyName = x.Company.CompanyName,
                IntroDescription = x.IntroDescription,
                Duration = x.Duration,
                QuestionCount = x.ExamQuestions.Count,
                LimitRate = x.LimitRate,
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundException();

        return data;
    }

    //Sirketin butun suallari ve cavablarini gormesi ucun
    public async Task<GetExamQuestionsDetailDto> GetExamQuestionsAsync(Guid examId)
    {
        var examQuestions = await _context.Exams
            .AsNoTracking()
            .Where(x => x.Id == examId && x.Company.UserId == _currentUser.UserGuid)
            .Select(exam => new GetExamQuestionsDetailDto
            {
                TotalQuestions = exam.ExamQuestions.Count,
                LimitRate = exam.LimitRate,
                Duration = exam.Duration,
                Questions = exam.ExamQuestions.OrderBy(x => x.Question.Order).Select(eq => new QuestionPublicDto
                {
                    Id = eq.Question.Id,
                    Title = eq.Question.Title,
                    Image = eq.Question.Image != null ? $"{_currentUser.BaseUrl}/companyFiles/{eq.Question.Image}" : null,
                    QuestionType = eq.Question.QuestionType,
                    IsRequired = eq.Question.IsRequired,
                    Order = eq.Question.Order,
                    Answers = eq.Question.Answers!.Select(a => new AnswerPublicDto
                    {
                        Id = a.Id,
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                }).ToList()
            }).FirstOrDefaultAsync() ?? throw new NotFoundException();

        return examQuestions;
    }

    //Isaxtaranin butun suallari gormesi ucun
    public async Task<GetExamQuestionsDetailDto> GetExamQuestionsForUserAsync(Guid examId)
    {
        var examQuestions = await _context.Exams
            .AsNoTracking()
            .Where(x => x.Id == examId)
            .Select(exam => new GetExamQuestionsDetailDto
            {
                TotalQuestions = exam.ExamQuestions.Count,
                LimitRate = exam.LimitRate,
                Duration = exam.Duration,
                Questions = exam.ExamQuestions.OrderBy(x => x.Question.Order).Select(eq => new QuestionPublicDto
                {
                    Id = eq.Question.Id,
                    Title = eq.Question.Title,
                    Image = eq.Question.Image != null ? $"{_currentUser.BaseUrl}/companyFiles/{eq.Question.Image}" : null,
                    QuestionType = eq.Question.QuestionType,
                    IsRequired = eq.Question.IsRequired,
                    Order = eq.Question.Order,
                    Answers = eq.Question.Answers!.Select(a => new AnswerPublicDto
                    {
                        Id = a.Id,
                        Text = a.Text,
                    }).ToList()
                }).ToList()
            }).FirstOrDefaultAsync() ?? throw new NotFoundException();

        return examQuestions;
    }

    public async Task<SubmitExamResultDto> EvaluateExamAnswersAsync(SubmitExamAnswersDto dto)
    {
        var userGuid = _currentUser.UserGuid ?? throw new InvalidOperationException("UserId can not be null");
        var exam = await _context.Exams
            .Include(e => e.ExamQuestions)
            .ThenInclude(eq => eq.Question)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(x => x.Id == dto.ExamId)
            ?? throw new NotFoundException();

        UserAnswerDto? userAnswer = new();
        List<Guid>? correctAnswers = new();

        var answerResults = exam.ExamQuestions
            .Select(eq =>
            {
                userAnswer = dto.Answers.FirstOrDefault(a => a.QuestionId == eq.Question.Id);
                if (userAnswer == null) return false;

                correctAnswers = eq.Question.Answers!
                    .Where(a => a.IsCorrect ?? false)
                    .Select(a => a.Id)
                    .ToList();

                return eq.Question.QuestionType switch
                {
                    QuestionType.MultipleChoice or QuestionType.SingleChoice =>
                        userAnswer.AnswerIds?.Count == correctAnswers.Count &&
                        !userAnswer.AnswerIds.Except(correctAnswers).Any(),

                    QuestionType.OpenEnded =>
                        !string.IsNullOrEmpty(userAnswer.Text) &&
                        string.Equals(
                            userAnswer.Text.Trim(),
                            eq.Question.Answers!.FirstOrDefault(a => a.IsCorrect ?? false)?.Text?.Trim(),
                            StringComparison.OrdinalIgnoreCase
                        ),

                    _ => false
                };
            })
            .ToList();

        int trueCount = answerResults.Count(isCorrect => isCorrect);
        int falseCount = answerResults.Count(isCorrect => !isCorrect);

        int totalQuestions = exam.ExamQuestions.Count;
        float resultRate = totalQuestions > 0 ? (float)(trueCount / totalQuestions) * 100 : 0;
        bool isPassed = resultRate >= exam.LimitRate;

        var userExam = new UserExam
        {
            UserId = userGuid,
            ExamId = dto.ExamId,
            VacancyId = dto.VacancyId,
            TrueAnswerCount = (byte)trueCount,
            FalseAnswerCount = (byte)falseCount,
            TotalPercent = resultRate
        };

        await _context.UserExams.AddAsync(userExam);
        await _context.SaveChangesAsync();

        return new SubmitExamResultDto
        {
            TrueAnswerCount = (byte)trueCount,
            FalseAnswerCount = (byte)falseCount,
            ResultRate = resultRate,
            IsPassed = isPassed
        };
    }

    public async Task<UserExamDetailDto> GetUserExamAsync(Guid examId, Guid userId)
    {
        var data = await _context.UserExams.Where(x => x.ExamId == examId && x.UserId == userId && x.Exam.Company.UserId == _currentUser.UserGuid)
            .Select(x => new UserExamDetailDto
            {
                TrueAnswerCount = x.TrueAnswerCount,
                FalseAnswerCount = x.FalseAnswerCount,
                TotalPercent = x.TotalPercent
            }).FirstOrDefaultAsync() ?? throw new NotFoundException();

        return data;
    }
}
