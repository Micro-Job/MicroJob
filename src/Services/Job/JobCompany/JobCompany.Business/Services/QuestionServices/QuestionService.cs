using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Business.Services.AnswerServices;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace JobCompany.Business.Services.QuestionServices
{
    public class QuestionService(IFileService fileService, JobCompanyDbContext context, IAnswerService answerService) : IQuestionService
    {
        /// <summary> Question yaradılması tekli method + answers </summary>
        public async Task CreateQuestionAsync(QuestionCreateDto dto, CreateListAnswersDto dtos)
        {
            FileDto fileResult = dto.Image != null
                ? await fileService.UploadAsync(FilePaths.document, dto.Image)
                : new FileDto();

            var question = new Question
            {
                Title = dto.Title,
                Image = dto.Image != null ? $"{fileResult.FilePath}/{fileResult.FileName}" : null,
                QuestionType = dto.QuestionType,
                IsRequired = dto.IsRequired,
            };

            var questionId = question.Id.ToString();

            await context.AddAsync(question);

            var answers = await answerService.CreateBulkAnswerAsync(dtos.Answers, questionId);
            question.Answers = answers;
        }


        ///<summary> Question yaradılması bulk method + answers_bulk /// </summary>
        public async Task<ICollection<Question>> CreateBulkQuestionAsync(ICollection<QuestionCreateDto> dtos, string examId)
        {
            var guidExam = Guid.Parse(examId);

            var tasks = dtos.Select(async dto =>
            {
                FileDto fileResult = dto.Image != null
                    ? await fileService.UploadAsync(FilePaths.document, dto.Image)
                    : new FileDto();

                var question = new Question
                {
                    Title = dto.Title,
                    Image = dto.Image != null ? $"{fileResult.FilePath}/{fileResult.FileName}" : null,
                    QuestionType = dto.QuestionType,
                    IsRequired = dto.IsRequired
                };

                var questionId = question.Id.ToString();

                if (dto.Answers != null && dto.Answers.Any())
                {
                    question.Answers = await answerService.CreateBulkAnswerAsync(dto.Answers, questionId);
                }

                return question;
            });

            var questionsToAdd = await Task.WhenAll(tasks);

            await context.Questions.AddRangeAsync(questionsToAdd);

            return questionsToAdd;
        }
    }
}