using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Business.Services.QuestionServices;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace JobCompany.Business.Services.ExamServices
{
    public class ExamService(JobCompanyDbContext context, IFileService fileService, IQuestionService questionService, IHttpContextAccessor contextAccessor) : IExamService
    {
        private readonly JobCompanyDbContext _context = context;
        private readonly IQuestionService _questionService = questionService;
        public async Task<Guid> CreateExamAsync(CreateExamDto dto)
        {
            var exam = new Exam
            {
                IntroDescription = dto.IntroDescription,
                LastDescription = dto.LastDescription,
            };
            await _context.Exams.AddAsync(exam);
            var examId = exam.Id.ToString();

            var questions = await _questionService.CreateBulkQuestionAsync(dto.Questions, examId);
            exam.Questions = questions;

            await _context.SaveChangesAsync();
            return exam.Id;
        }

        public async Task<GetExamByIdDto> GetExamByIdAsync(string examId, byte step)
        {
            var examGuid = Guid.Parse(examId);
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == examGuid)
                ?? throw new NotFoundException<Exam>();
            var examDto = new GetExamByIdDto
            {
                IntroDescription = step == 1 ? exam.IntroDescription : null,
                CurrentStep = step,
                LastDescription = exam.LastDescription,
                Result = exam.Result,
                Questions = step == 2
            ? exam.Questions.Select(q => new Question
            {
                Id = q.Id,
                Title = q.Title,
                QuestionType = q.QuestionType,
                Answers = q.Answers?.Select(a => new Answer
                {
                    Id = a.Id,
                    Text = a.Text
                }).ToList()
            }).ToList()
            : null
            };
            return examDto;
        }
    }
}