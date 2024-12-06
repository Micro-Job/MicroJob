using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.ExamDtos.AnswerDtos;
using JobCompany.Business.Dtos.ExamDtos.QuestionDtos;
using JobCompany.Business.Exceptions.ExamExceptions;
using JobCompany.Business.Services.ExamServices.AnswerServices;
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
        readonly IAnswerService _answerService;

        public QuestionService(IFileService fileService, JobCompanyDbContext context, IAnswerService answerService)
        {
            _fileService = fileService;
            _context = context;
            _answerService = answerService;
        }
        /// <summary> Question yaradılması tekli method + answers </summary>
        public async Task CreateQuestionAsync(QuestionCreateDto dto, CreateListAnswersDto dtos)
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

            var questionId = question.Id.ToString();

            await _context.AddAsync(question);

            var answers = await _answerService.CreateBulkAnswerAsync(dtos.Answers, questionId);
            question.Answers = answers;
        }


        ///<summary> Question yaradılması bulk method + answers </summary>
        public async Task<ICollection<Question>> CreateBulkQuestionAsync(ICollection<QuestionCreateDto> dtos, string examId)
        {
            var guidExam = Guid.Parse(examId);

            var questionsToAdd = new List<Question>();

            foreach (var dto in dtos)
            {
                FileDto fileResult = dto.Image != null
                    ? await _fileService.UploadAsync(FilePaths.document, dto.Image)
                    : new FileDto();

                var question = new Question
                {
                    ExamId = guidExam,
                    Title = dto.Title,
                    Image = dto.Image != null ? $"{fileResult.FilePath}/{fileResult.FileName}" : null,
                    QuestionType = dto.QuestionType,
                    IsRequired = dto.IsRequired
                };

                var questionId = question.Id.ToString();

                if (dto.Answers != null && dto.Answers.Any())
                {
                    var answers = await _answerService.CreateBulkAnswerAsync(dto.Answers, questionId);
                    question.Answers = answers;
                }

                questionsToAdd.Add(question);
            }

            await _context.Questions.AddRangeAsync(questionsToAdd);
            return questionsToAdd;
        }
    }
}