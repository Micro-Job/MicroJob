using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Job.Business.Dtos.FileDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.ExternalServices;
using Job.Business.Statics;
using Job.Core.Entities;
using Job.DAL.Contexts;

namespace Job.Business.Services.Resume
{
    public class ResumeService : IResumeService
    {
        readonly AppDbContext _context;
        readonly IFileService _fileService;

        public ResumeService(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }


        public async Task CreateAsync(ResumeCreateDto resumeCreateDto)
        {
            FileDto fileResult = new FileDto();
            var userId = Guid.Parse(resumeCreateDto.UserId);

            if (resumeCreateDto.UserPhoto != null) fileResult = await _fileService.UploadAsync(FilePaths.image, resumeCreateDto.UserPhoto);

            var resume = new Core.Entities.Resume
            {
                UserId = userId,
                FatherName = resumeCreateDto.FatherName,
                Position = resumeCreateDto.Position,
                IsDriver = resumeCreateDto.IsDriver,
                IsMarried = resumeCreateDto.IsMarried,
                IsCitizen = resumeCreateDto.IsCitizen,
                Gender = resumeCreateDto.Gender,
                Adress = resumeCreateDto.Adress,
                BirthDay = resumeCreateDto.BirthDay,
                UserPhoto = $"{fileResult.FilePath}/{fileResult.FileName}"
            };
            await _context.Resumes.AddAsync(resume);
            await _context.SaveChangesAsync();
        }
    }
}
