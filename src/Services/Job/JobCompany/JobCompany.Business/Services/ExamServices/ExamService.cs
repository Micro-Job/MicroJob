using System.Security.Claims;
using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Business.Exceptions.UserExceptions;
using JobCompany.Business.Services.QuestionServices;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.ExternalServices.FileService;

namespace JobCompany.Business.Services.ExamServices
{
    public class ExamService(JobCompanyDbContext context, IFileService fileService, IQuestionService questionService, IHttpContextAccessor _contextAccessor) : IExamService
    {
        private readonly JobCompanyDbContext _context = context;
        private readonly IQuestionService _questionService = questionService;
        private readonly Guid userGuid = Guid.Parse(_contextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.Sid)?.Value ?? throw new UserIsNotLoggedInException());
        public async Task<Guid> CreateExamAsync(CreateExamDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var company = await _context.Companies.FirstOrDefaultAsync(a => a.UserId == userGuid)
            ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>();
            try
            {
                var exam = new Exam
                {
                    Title = dto.Title,
                    IntroDescription = dto.IntroDescription,
                    LastDescription = dto.LastDescription,
                    Result = dto.Result,
                    CompanyId = company.Id,
                    IsTemplate = dto.IsTemplate
                };

                await _context.Exams.AddAsync(exam);

                await _context.SaveChangesAsync();

                var examId = exam.Id.ToString();

                var questions = await _questionService.CreateBulkQuestionAsync(dto.Questions, examId);
                // exam.Questions = questions;

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


        public async Task<GetExamByIdDto> GetExamByIdAsync(string examId, byte step)
        {
            var examGuid = Guid.Parse(examId);

            var exam = await _context.Exams
                // .Include(e => e.Questions)
                // .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(e => e.Id == examGuid)
                ?? throw new SharedLibrary.Exceptions.NotFoundException<Exam>();

            // var orderedQuestions = exam.Questions.OrderBy(q => q.Id).ToList();

            // var stepQuestion = orderedQuestions.Skip(step - 1).Take(1).ToList();

            var examDto = new GetExamByIdDto
            {
                IntroDescription = exam.IntroDescription,
                CurrentStep = step,
                LastDescription = exam.LastDescription,
                Result = exam.Result,
                // Questions = stepQuestion.Select(q => new QuestionDetailDto
                // {
                //     Title = q.Title,
                //     Image = q.Image,
                //     QuestionType = q.QuestionType,
                //     IsRequired = q.IsRequired,
                //     Answers = q.Answers.Select(a => new AnswerDetailDto
                //     {
                //         Text = a.Text,
                //         IsCorrect = a.IsCorrect,
                //     }).ToList()
                // }).ToList()
            };

            return examDto;
        }
    }
}