using AuthService.Business.Dtos;
using AuthService.Business.Exceptions.UserException;
using AuthService.Business.Services.CurrentUser;
using AuthService.Core.Entities;
using AuthService.DAL.Contexts;
using Job.Business.Dtos.FileDtos;
using Job.Business.ExternalServices;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Business.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly ICurrentUser _currentUser;
        private readonly string _currentUserId;
        private readonly Guid _currentUserGuid;

        public UserService(AppDbContext context, IFileService fileService, ICurrentUser currentUser)
        {
            _context = context;
            _fileService = fileService;
            _currentUser = currentUser;
            _currentUserId = _currentUser.UserId ?? throw new UserNotLoggedInException();
            _currentUserGuid = Guid.Parse(_currentUserId);
        }

        public async Task<UserInformationDto> GetUserInformationAsync()
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == _currentUserGuid).Select(x => new UserInformationDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    MainPhoneNumber = x.MainPhoneNumber,
                    Image = $"{_currentUser.BaseUrl}/{x.Image}",
                })
                ?? throw new UserNotFoundException();

            return user;
        }

        public async Task<UserUpdateResponseDto> UpdateUserInformationAsync(UserUpdateDto dto)
        {
            var userQuery = _context.Users.AsQueryable();
            var user = await userQuery
                       .FirstOrDefaultAsync(u => u.Id == _currentUserGuid)
                        ?? throw new UserNotFoundException();
            var isEmailTaken = await userQuery
                .FirstOrDefaultAsync(u => u.Id != user.Id &&
                                     u.Email == dto.Email.Trim());

            if (isEmailTaken is not null)
            {
                throw new UserExistException("Email mövcuddur!");
            }

            var isPhoneTaken = await userQuery
                .FirstOrDefaultAsync(u => u.Id != user.Id && u.MainPhoneNumber == dto.MainPhoneNumber.Trim());

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
                Image = user.Image
            };
        }

        public async Task<UserProfileImageUpdateResponseDto> UpdateUserProfileImageAsync(UserProfileImageUpdateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _currentUserGuid).Select(x => new User
            {
                Image = x.Image,
                Id = x.Id
            }) ?? throw new UserNotFoundException();
            if (user.Image is not null) _fileService.DeleteFile(user.Image);
            FileDto image = await _fileService.UploadAsync("wwwroot/images", dto.Image);
            var imageUrl = image.FilePath;
            user.Image = imageUrl;

            await _context.SaveChangesAsync();

            return new UserProfileImageUpdateResponseDto
            {
                UserId = user.Id,
                ImageUrl = imageUrl
            };
        }
    }
}