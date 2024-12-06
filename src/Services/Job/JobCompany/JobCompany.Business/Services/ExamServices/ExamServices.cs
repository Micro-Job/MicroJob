using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Business.Services.ExamServices.QuestionServices;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
            var exam = new Exam
            {
                LogoUrl = dto.Logo != null
                    ? $"{_fileService.UploadAsync(FilePaths.document, dto.Logo).Result.FilePath}/{_fileService.UploadAsync(FilePaths.document, dto.Logo).Result.FileName}"
                    : throw new Exception("Sekil daxil edin!"),
                TemplateId = dto.TemplateId,
                IntroDescription = dto.IntroDescription,
                LastDescription = dto.LastDescription,
                Result = dto.Result
            };
            var examId = exam.Id.ToString();
            var questions = await _questionService.CreateBulkQuestionAsync(dto.Questions, examId);
            exam.Questions = questions;
            await _context.Exams.AddAsync(exam);
            await _context.SaveChangesAsync();
        }

        public async Task<GetExamByIdDto> GetExamByIdAsync(string examId)
        {
            var examGuid = Guid.Parse(examId);
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == examGuid)
                ?? throw new NotFoundException<Exam>();
            var examDto = new GetExamByIdDto
            {
                LogoUrl = $"{_baseUrl}/{exam.LogoUrl}",
                IntroDescription = exam.IntroDescription,
                LastDescription = exam.LastDescription,
                Result = exam.Result,
                Questions = exam.Questions
            };

            throw new NotImplementedException();
        }
    }
}