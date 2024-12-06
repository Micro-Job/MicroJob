using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Business.Exceptions.ExamExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace JobCompany.Business.Services.QuestionServices
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
    }
}