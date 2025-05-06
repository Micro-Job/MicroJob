using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Business.Services.AnswerServices;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace JobCompany.Business.Services.QuestionServices
{
    public class QuestionService(
        IFileService fileService,
        JobCompanyDbContext context,
        IAnswerService answerService
    ) : IQuestionService
    {
        /// <summary> Question yaradılması tekli method + answers </summary>
        public async Task CreateQuestionAsync(QuestionCreateDto dto, CreateListAnswersDto dtos)
        {
            FileDto fileResult =
                dto.Image != null
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
        public async Task<ICollection<Question>> CreateBulkQuestionAsync(ICollection<QuestionCreateDto> dtos)
        {
            var tasks = dtos.Select(async dto =>
            {
                FileDto fileResult =
                    dto.Image != null
                        ? await fileService.UploadAsync(FilePaths.document, dto.Image)
                        : new FileDto();

                var question = new Question
                {
                    Title = dto.Title,
                    Image =
                        dto.Image != null ? $"{fileResult.FilePath}/{fileResult.FileName}" : null,
                    QuestionType = dto.QuestionType,
                    IsRequired = dto.IsRequired,
                    Order = dto.Order
                };

                var questionId = question.Id.ToString();

                if (dto.Answers != null && dto.Answers.Any())
                {
                    question.Answers = await answerService.CreateBulkAnswerAsync(
                        dto.Answers,
                        questionId
                    );
                }

                return question;
            });

            var questionsToAdd = await Task.WhenAll(tasks);

            await context.Questions.AddRangeAsync(questionsToAdd);

            return questionsToAdd;
        }

        public async Task<ICollection<Guid>> UpdateBulkQuestionsAsync(Guid examId, ICollection<QuestionUpdateDto> dtos)
        {
            var existingQuestions = await context.Questions
                .Include(q => q.Answers)
                .Where(q => q.ExamQuestions.Any(eq => eq.ExamId == examId))
                .ToListAsync();

            var updatedQuestionIds = new List<Guid>(); // həm update olunan həm də yeni əlavə olunan sualların id-ləri üçün siyahı. Sonda bu id-ləri qaytarırıq
            var questionsToAdd = new List<Question>(); // Yeni sualların əlavə olunması üçün siyahı
            var examquestionsToAdd = new List<ExamQuestion>();

            var existingQuestionsDict = existingQuestions.ToDictionary(q => q.Id);

            foreach (var dto in dtos)
            {
                if (dto.Id.HasValue) // Əgər dto-dakı sualın id-si varsa deməli sual update olunacaq
                {
                    if (!existingQuestionsDict.TryGetValue(dto.Id.Value, out var question))
                        continue; // Sual tapılmadı, davam et

                    question.Title = dto.Title;
                    question.QuestionType = dto.QuestionType;
                    question.IsRequired = dto.IsRequired;
                    question.Order = dto.Order;

                    if (dto.IsImageDeleted) // Əgər user şəkili silmək istəyibsə
                    {
                        if (!string.IsNullOrWhiteSpace(question.Image)) // Sualın əvvəldən şəkli varsa 
                            fileService.DeleteFile(question.Image); // Köhnə şəkili sil
                        
                        question.Image = null; // db-də şəkili null-a set edirik
                    }
                    else if (dto.Image != null) // Əgər user yeni şəkil yükləyibsə
                    {
                        if (!string.IsNullOrWhiteSpace(question.Image))
                            fileService.DeleteFile(question.Image); // Köhnə şəkili sil

                        FileDto fileResult = await fileService.UploadAsync(FilePaths.document, dto.Image); // Yeni şəkili yüklə
                        question.Image = $"{fileResult.FilePath}/{fileResult.FileName}"; // Yeni şəkilin path-ni property-yə set et
                    }

                    updatedQuestionIds.Add(question.Id);
                }
                else //Əgər dto-dakı sualın id-si yoxdursa deməli sual yeni əlavə olunub. Sualı və cavabları yaradır
                {
                    var newQuestion = new Question
                    {
                        Title = dto.Title,
                        QuestionType = dto.QuestionType,
                        IsRequired = dto.IsRequired,
                        Order = dto.Order,
                        Answers = dto.Answers?.Select(a => new Answer
                        {
                            Text = a.Text,
                            IsCorrect = a.IsCorrect,
                        }).ToList() ?? []
                    };

                    var examQuestion = new ExamQuestion
                    {
                        ExamId = examId,
                        Question = newQuestion,
                    };

                    if (dto.Image != null)
                    {
                        FileDto fileResult =
                            await fileService.UploadAsync(FilePaths.document, dto.Image);
                        newQuestion.Image = $"{fileResult.FilePath}/{fileResult.FileName}";
                    }
                    
                    questionsToAdd.Add(newQuestion);
                    examquestionsToAdd.Add(examQuestion);
                    updatedQuestionIds.Add(newQuestion.Id);
                }
            }

            // dto-da olan id-lərlə mövcud sualların id-lərini müqayisə edir və silinməsi lazım olanları tapır
            var dtoIds = dtos.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToHashSet();
            var questionsToRemove = existingQuestions.Where(q => !dtoIds.Contains(q.Id)).ToList();
            

            await answerService.UpdateBulkAnswersAsync(existingQuestions, dtos); 
            
            if (questionsToRemove.Count != 0)
                context.Questions.RemoveRange(questionsToRemove);

            if (questionsToAdd.Count != 0)
                await context.Questions.AddRangeAsync(questionsToAdd);

            if (examquestionsToAdd.Count != 0)
                await context.ExamQuestions.AddRangeAsync(examquestionsToAdd);

            return updatedQuestionIds;
        }
    }
}
