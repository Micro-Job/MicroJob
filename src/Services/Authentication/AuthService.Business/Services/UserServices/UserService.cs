using AuthService.Business.Dtos;
using AuthService.Business.Exceptions.UserException;
using AuthService.Business.Services.CurrentUser;
using AuthService.DAL.Contexts;
using Job.Business.ExternalServices;
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
            var user = await _context.Users.Include(r => r.UserStatus)
                .Where(u => u.Id == Guid.Parse(_currentUserId))
                .AsNoTracking()
                .FirstOrDefaultAsync()
                ?? throw new UserNotFoundException();

            return new UserInformationDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MainPhoneNumber = user.MainPhoneNumber,
                Image = user.Image,
            };
        }

        public async Task<UserUpdateResponseDto> UpdateUserInformation(UserUpdateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(_currentUserId))
                ?? throw new UserNotFoundException();
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.MainPhoneNumber = dto.MainPhoneNumber;
            _context.Users.Update(user);
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(_currentUserId))
                ?? throw new UserNotFoundException();
            var image = await _fileService.UploadAsync("/image", dto.Image);
            var imageUrl = image.FilePath;
            user.Image = imageUrl;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new UserProfileImageUpdateResponseDto
            {
                UserId = user.Id,
                ImageUrl = imageUrl
            };
        }
    }
}