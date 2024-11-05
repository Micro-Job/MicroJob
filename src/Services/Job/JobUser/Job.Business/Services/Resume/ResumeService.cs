using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Job.Business.Dtos.FileDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Exceptions.Common;
using Job.Business.Exceptions.UserExceptions;
using Job.Business.ExternalServices;
using Job.Business.Statics;
using Job.Core.Entities;
using Job.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Job.Business.Services.Resume
{
    public class ResumeService : IResumeService
    {
        readonly JobDbContext _context;
        readonly IFileService _fileService;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly Guid _userId;

        public ResumeService(JobDbContext context, IFileService fileService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userId == null) throw new UserIsNotLoggedInException();
            _userId = Guid.Parse(userId.Value);
        }

        public async Task CreateAsync(ResumeCreateDto resumeCreateDto)
        {
            FileDto fileResult = new FileDto();

            if (resumeCreateDto.UserPhoto != null)
                fileResult = await _fileService.UploadAsync(FilePaths.image, resumeCreateDto.UserPhoto);

            var resume = new Core.Entities.Resume
            {
                UserId = _userId,
                FatherName = resumeCreateDto.FatherName,
                Position = resumeCreateDto.Position,
                IsDriver = resumeCreateDto.IsDriver,
                IsMarried = resumeCreateDto.IsMarried,
                IsCitizen = resumeCreateDto.IsCitizen,
                Gender = resumeCreateDto.Gender,
                Adress = resumeCreateDto.Adress,
                BirthDay = resumeCreateDto.BirthDay,
                UserPhoto = resumeCreateDto.UserPhoto != null 
                    ? $"{fileResult.FilePath}/{fileResult.FileName}"
                    : null
            };

            await _context.Resumes.AddAsync(resume);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ResumeListDto>> GetAllAsync()
        {
            var resumes = await _context.Resumes.ToListAsync();
            var resumeList = resumes.Select(r => new ResumeListDto
            {
                UserId = r.UserId,
                FatherName = r.FatherName,
                Position = r.Position,
                UserPhoto = r.UserPhoto,
                IsDriver = r.IsDriver,
                IsMarried = r.IsMarried,
                IsCitizen = r.IsCitizen,
                Gender = r.Gender,
                Adress = r.Adress,
                BirthDay = r.BirthDay
            });

            return resumeList;
        }

        public async Task<ResumeDetailItemDto> GetByIdAsync(string id)
        {
            var resume = await _context.Resumes.FindAsync(Guid.Parse(id));
            if (resume is null) throw new NotFoundException<Core.Entities.Resume>();
            var resumeDetail = new ResumeDetailItemDto
            {
                UserId = resume.UserId,
                FatherName = resume.FatherName,
                Position = resume.Position,
                UserPhoto = resume.UserPhoto,
                IsDriver = resume.IsDriver,
                IsMarried = resume.IsMarried,
                IsCitizen = resume.IsCitizen,
                Gender = resume.Gender,
                Adress = resume.Adress,
                BirthDay = resume.BirthDay,
            };
            return resumeDetail;
        }

        public async Task UpdateAsync(ResumeUpdateDto resumeUpdateDto)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.UserId == _userId)
                            ?? throw new NotFoundException<Core.Entities.Resume>();

            resume.FatherName = resumeUpdateDto.FatherName;
            resume.Position = resumeUpdateDto.Position;
            resume.IsDriver = resumeUpdateDto.IsDriver;
            resume.IsMarried = resumeUpdateDto.IsMarried;
            resume.IsCitizen = resumeUpdateDto.IsCitizen;
            resume.Gender = resumeUpdateDto.Gender;
            resume.Adress = resumeUpdateDto.Adress;
            resume.BirthDay = resumeUpdateDto.BirthDay;

            if (resumeUpdateDto.UserPhoto != null)
            {
                if (resume.UserPhoto != null)
                {
                    _fileService.DeleteFile(resume.UserPhoto);
                };
                var fileResult = await _fileService.UploadAsync(FilePaths.image, resumeUpdateDto.UserPhoto);
                resume.UserPhoto = $"{fileResult.FilePath}/{fileResult.FileName}";
            }

            _context.Resumes.Update(resume);
            await _context.SaveChangesAsync();
        }
    }
}
