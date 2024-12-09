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
    public class ExamServices(JobCompanyDbContext context, IFileService fileService, IQuestionService questionService, IHttpContextAccessor contextAccessor) : IExamServices
    {
        private readonly JobCompanyDbContext _context = context;
        private readonly IFileService _fileService = fileService;
        private readonly IQuestionService _questionService = questionService;
        private readonly string? _baseUrl = $"{contextAccessor?.HttpContext?.Request.Scheme}://{contextAccessor?.HttpContext?.Request.Host.Value}{contextAccessor?.HttpContext?.Request.PathBase.Value}";
        public async Task CreateExamAsync(CreateExamDto dto)
        {
            FileDto fileResult = dto.Logo != null
                ? await _fileService.UploadAsync(FilePaths.lowImage, dto.Logo)
                : new FileDto();
            var exam = new Exam
            {
                LogoUrl = dto.Logo != null
                    ? $"{fileResult.FilePath}/{fileResult.FileName}"
                    : throw new Exception("Sekil daxil edin!"),
                TemplateId = dto.TemplateId,
                IntroDescription = dto.IntroDescription,
                LastDescription = dto.LastDescription,
            };
            await _context.Exams.AddAsync(exam);
            var examId = exam.Id.ToString();

            var questions = await _questionService.CreateBulkQuestionAsync(dto.Questions, examId);
            exam.Questions = questions;
            
            await _context.SaveChangesAsync();
        }

        public async Task<GetExamByIdDto> GetExamByIdAsync(string examId, byte step)
        {
            var examGuid = Guid.Parse(examId);
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == examGuid)
                ?? throw new NotFoundException<Exam>();
            var examDto = new GetExamByIdDto
            {
                LogoUrl = $"{_baseUrl}/{exam.LogoUrl}",
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
                Time = q.Time,
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