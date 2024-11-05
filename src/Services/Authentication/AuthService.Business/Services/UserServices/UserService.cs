using AuthService.Business.Dtos;
using AuthService.Business.Exceptions.UserException;
using AuthService.DAL.Contexts;
using Job.Business.Dtos.FileDtos;
using Job.Business.ExternalServices;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Business.Services.UserServices
{
    public class UserService(AppDbContext context, IFileService fileService) : IUserService
    {
        private readonly AppDbContext _context = context;
        private readonly IFileService _fileService = fileService;

        public async Task<UserInformationDto> GetUserInformationAsync()
        {
            var user = await _context.Users.Include(r => r.UserStatus)
                .Where(u => u.Id == Guid.NewGuid())
                .AsNoTracking()
                .FirstOrDefaultAsync()
                ?? throw new UserNotFoundException();
            var dto = new UserInformationDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MainPhoneNumber = user.MainPhoneNumber,
                Image = user.Image,
            };
            return dto;
        }

        public async Task<UserUpdateResponseDto> UpdateUserInformation(UserUpdateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.Id)
                ?? throw new UserNotFoundException();
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.MainPhoneNumber = dto.MainPhoneNumber;
            _context.Update(dto);
            await _context.SaveChangesAsync();

            return new UserUpdateResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MainPhoneNumber = user.MainPhoneNumber,
                Image = string.Empty
            };
        }

        public async Task<UserProfileImageUpdateResponseDto> UpdateUserProfileImage(UserProfileImageUpdateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId)
                ?? throw new UserNotFoundException();
            FileDto image = await _fileService.UploadAsync("/image", dto.Image);
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