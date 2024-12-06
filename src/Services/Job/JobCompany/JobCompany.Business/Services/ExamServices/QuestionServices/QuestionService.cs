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

        /// <summary> Question yaradılması coxlu </summary>
        public async Task<ICollection<Question>> CreateBulkQuestionAsync(ICollection<QuestionCreateDto> dtos, string examId)
        {
            var guidExam = Guid.Parse(examId);
            var questionsToAdd = dtos.Select(dto => new Question
            {
                ExamId = guidExam,
                Title = dto.Title,
                Image = dto.Image != null
                    ? $"{_fileService.UploadAsync(FilePaths.document, dto.Image).Result.FilePath}/{_fileService.UploadAsync(FilePaths.document, dto.Image).Result.FileName}"
                    : null,
                QuestionType = dto.QuestionType,
                IsRequired = dto.IsRequired,
            }).ToList();

            await _context.Questions.AddRangeAsync(questionsToAdd);
            await _context.SaveChangesAsync();

            return questionsToAdd;
        }

        /// <summary> Question yaradılması tekli </summary>
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