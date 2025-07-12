using FluentValidation;
using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using SharedLibrary.Enums;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.ResumeDtos
{
    public record ResumeCreateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? PositionId { get; set; } //Əgər position varsa db-də onun id-si yazılmalıdır
        public string? Position { get; set; } //Əgər position yoxdursa bu adda olan bir position yaradılır db-də
        public Guid? ParentPositionId { get; set; }
        public IFormFile? UserPhoto { get; set; }
        public Driver IsDriver { get; set; }
        public FamilySituation IsMarried { get; set; }
        public Citizenship IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public Military MilitarySituation { get; set; }
        public bool IsMainNumber { get; set; }
        public bool IsMainEmail { get; set; }
        public bool IsPublic { get; set; }
        public bool IsAnonym { get; set; }
        public string? ResumeEmail { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }

        public ICollection<Guid>? SkillIds { get; set; }

        public ICollection<CertificateCreateDto>? Certificates { get; set; }
        public ICollection<NumberCreateDto> PhoneNumbers { get; set; }
        public ICollection<ExperienceCreateDto>? Experiences { get; set; }
        public ICollection<EducationCreateDto>? Educations { get; set; }
        public ICollection<LanguageCreateDto>? Languages { get; set; }
    }

    public class ResumeCreateDtoValidator : AbstractValidator<ResumeCreateDto>
    {
        public ResumeCreateDtoValidator()
        {
            //RuleFor(x => x.FatherName)
            //    .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
            //    .MaximumLength(50).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_50"));

            RuleFor(x => x.Position)
                //.NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(100).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"));

            RuleFor(x => x.Adress)
                .MaximumLength(200).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_200"));

            RuleFor(x => x.BirthDay.Date.Year)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .LessThanOrEqualTo(DateTime.Now.Date.Year - 16).WithMessage(MessageHelper.GetMessage("BIRTHDAY_MUST_BE_IN_THE_PAST"));

            When(x => x.UserPhoto != null, () =>
            {
                RuleFor(x => x.UserPhoto)
                    .Must(photo =>
                    {
                        // CheckFileSize metodundan gelen mesajı al
                        string? errorMessage = FileService.CheckFileSizeForValidation(photo.Length, FileType.Image);

                        // Eğer errorMessage boş değilse (yani bir hata varsa) FluentValidation'a bildir
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            // Bu ValidationRule'ın hata mesajını belirle
                            // Aslında FromServices() kullanıp IValidator<IFormFile> tanımlamak daha iyi ama bu da çalışır.
                            // WithMessage metodunu burada kullanmayacağız, çünkü mesajı CheckFileSize'tan alıyoruz.
                            return false; // Validasyon başarısız oldu
                        }
                        return true; // Validasyon başarılı oldu
                    })
                    .WithMessage(""); // Hata mesajını buradan al
            });

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleForEach(x => x.Certificates)
                .SetValidator(new CertificateCreateDtoValidator());

            RuleForEach(x => x.Experiences)
                .SetValidator(new ExperienceCreateDtoValidator());

            RuleForEach(x => x.Educations)
                .SetValidator(new EducationCreateDtoValidator());
        }
    }
}