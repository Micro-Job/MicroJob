using AuthService.Business.Dtos;
using AuthService.Business.Exceptions.UserException;
using AuthService.Business.Services.CurrentUser;
using AuthService.Core.Entities;
using AuthService.DAL.Contexts;
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

        public UserService(AppDbContext context, IFileService fileService, ICurrentUser currentUser)
        {
            _context = context;
            _fileService = fileService;
            _currentUser = currentUser;
            _currentUserId = _currentUser.UserId ?? throw new UserNotLoggedInException();
        }

        public async Task<UserInformationDto> GetUserInformationAsync()
        {
            var userGuid = Guid.Parse(_currentUserId);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userGuid).Select(x=> new UserInformationDto
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

        public async Task<UserUpdateResponseDto> UpdateUserInformation(UserUpdateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(_currentUserId))
                ?? throw new UserNotFoundException();

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

        public async Task<UserProfileImageUpdateResponseDto> UpdateUserProfileImage(UserProfileImageUpdateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(_currentUserId)).Select(x=> new User
            {
                Image = x.Image,
                Id = x.Id
            }) ?? throw new UserNotFoundException();

            var image = await _fileService.UploadAsync("/image", dto.Image);
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