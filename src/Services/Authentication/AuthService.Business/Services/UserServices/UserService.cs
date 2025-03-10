using AuthService.Business.Dtos;
using AuthService.Business.Exceptions.UserException;
using AuthService.Core.Entities;
using AuthService.Core.Enums;
using AuthService.DAL.Contexts;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using SharedLibrary.Statics;

namespace AuthService.Business.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly ICurrentUser _currentUser;

        public UserService(AppDbContext context, IFileService fileService, ICurrentUser currentUser)
        {
            _context = context;
            _fileService = fileService;
            _currentUser = currentUser;
        }

        /// <summary> Loginde olan User informasiyası </summary>
        public async Task<UserInformationDto> GetUserInformationAsync()
        {
            var user =
                await _context
                    .Users.FirstOrDefaultAsync(u => u.Id == _currentUser.UserGuid)
                    .Select(x => new UserInformationDto
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email,
                        MainPhoneNumber = x.MainPhoneNumber,
                        Image = x.Image != null ? $"{_currentUser.BaseUrl}/{x.Image}" : null,
                        UserRole = x.UserRole,
                        JobStatus = x.JobStatus,
                    }) ?? throw new UserNotFoundException();

            return user;
        }

        /// <summary> Logində olan userin informasiyasının update'si </summary>
        public async Task<UserUpdateResponseDto> UpdateUserInformationAsync(UserUpdateDto dto)
        {
            var userQuery = _context.Users.AsQueryable();
            var user =
                await userQuery.FirstOrDefaultAsync(u => u.Id == _currentUser.UserGuid)
                ?? throw new UserNotFoundException();
            var isEmailTaken = await userQuery.FirstOrDefaultAsync(u =>
                u.Id != user.Id && u.Email == dto.Email.Trim()
            );

            if (isEmailTaken is not null)
            {
                throw new UserExistException("Email mövcuddur!");
            }

            var isPhoneTaken = await userQuery.FirstOrDefaultAsync(u =>
                u.Id != user.Id && u.MainPhoneNumber == dto.MainPhoneNumber.Trim()
            );

            if (isPhoneTaken is not null)
            {
                throw new UserExistException("Bu nömrə sistemdə artıq mövcuddur!");
            }

            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.Email = dto.Email.Trim();
            user.MainPhoneNumber = dto.MainPhoneNumber.Trim();
            await _context.SaveChangesAsync();

            return new UserUpdateResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MainPhoneNumber = user.MainPhoneNumber,
                JobStatus = user.JobStatus,
            };
        }

        /// <summary> Logində olan userin şəkil update'si </summary>
        public async Task<UserProfileImageUpdateResponseDto> UpdateUserProfileImageAsync(UserProfileImageUpdateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.UserGuid)
                                                    ?? throw new UserNotFoundException();

            if (!string.IsNullOrEmpty(user.Image))
            {
                _fileService.DeleteFile(user.Image);
            }

            FileDto fileResult =
                dto.Image != null
                    ? await _fileService.UploadAsync(FilePaths.image, dto.Image)
                    : new FileDto();

            user.Image = $"{fileResult.FilePath}/{fileResult.FileName}";

            await _context.SaveChangesAsync();

            return new UserProfileImageUpdateResponseDto
            {
                UserId = user.Id,
                ImageUrl = user.Image,
            };
        }

        /// <summary> İstifadəçinin iş statusunun update olunması </summary>
        public async Task<JobStatus> UpdateUserJobStatusAsync(UserJobStatusUpdateDto dto)
        {
            var user =
                await _context.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.UserGuid)
                ?? throw new UserNotFoundException();

            user.JobStatus = dto.JobStatus;

            await _context.SaveChangesAsync();

            return user.JobStatus;
        }
    }
}
