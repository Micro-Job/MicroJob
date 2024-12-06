using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.ExamDtos.QuestionDtos;
using JobCompany.Business.Exceptions.ExamExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Exceptions;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace JobCompany.Business.Services.ExamServices.QuestionServices
{
    public class QuestionService : IQuestionService
    {
        readonly IFileService _fileService;
        readonly JobCompanyDbContext _context;

        public QuestionService(IFileService fileService, JobCompanyDbContext context)
        {
            _fileService = fileService;
            _context = context;
        }

        public async Task CreateQuestionAsync(QuestionCreateDto dto)
        {
            FileDto fileResult = dto.Image != null
                ? await _fileService.UploadAsync(FilePaths.document, dto.Image)
                : new FileDto();

            if (dto.QuestionType == Core.Enums.QuestionType.ImageBased && dto.Image == null) throw new InvalidQuestionException();

            var question = new Question
            {
                Title = dto.Title,
                Image = dto.Image != null ? $"{fileResult.FilePath}/{fileResult.FileName}" : null,
                QuestionType = dto.QuestionType,
                IsRequired = dto.IsRequired,
            };

            await _context.Questions.AddAsync(question);
        }

        public async Task DeleteQuestionAsync(string questionId)
        {
            var guidQuestionId = Guid.Parse(questionId);
            var question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == guidQuestionId)
             ?? throw new NotFoundException<Question>();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }
    }
}