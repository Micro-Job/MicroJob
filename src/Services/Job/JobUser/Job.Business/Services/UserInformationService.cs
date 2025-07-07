using Job.Business.Dtos.UserDtos;
using Job.Business.Exceptions.UserExceptions;
using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using SharedLibrary.Statics;

namespace Job.Business.Services;

public class UserInformationService(JobDbContext _context, ICurrentUser _currentUser, IFileService _fileService, IPublishEndpoint _publishEndpoint) 
{
    public async Task<JobStatus> UpdateUserJobStatusAsync(UserJobStatusUpdateDto dto)
    {
        var affectedRows = await _context.Users
            .Where(u => u.Id == _currentUser.UserGuid)
            .ExecuteUpdateAsync(builder => builder
            .SetProperty(u => u.JobStatus, dto.JobStatus));

        return dto.JobStatus;
    }

    public async Task<UserInformationDto> GetUserInformationAsync()
    {
        UserInformationDto user = await _context.Users.Where(x => x.Id == _currentUser.UserGuid)
            .Select(x=> new UserInformationDto
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Image = x.Image != null ? $"{_currentUser.BaseUrl}/userFiles/{x.Image}" : null,
                JobStatus = x.JobStatus,
                MainPhoneNumber = x.MainPhoneNumber,
                UserRole = UserRole.SimpleUser
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundUserException();

        return user;
    }

    public async Task<string?> UpdateUserInformationAsync(UpdateUserDto dto)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.UserGuid)
                        ?? throw new NotFoundUserException();

        if (dto.Image is not null)
        {
            if (!string.IsNullOrEmpty(user.Image))
            {
                _fileService.DeleteFile(user.Image);
                user.Image = null;
            }

            FileDto fileResult = dto.Image != null
                ? await _fileService.UploadAsync(FilePaths.image, dto.Image)
                : new FileDto();

            user.Image = $"{fileResult.FilePath}/{fileResult.FileName}";
        }
        else
        {
            user.Image = null;
        }

        dto.Email = dto.Email.Trim();
        dto.MainPhoneNumber = dto.MainPhoneNumber.Trim();

        await CheckUserExistAsync(dto.Email, dto.MainPhoneNumber);

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = dto.Email;
        user.MainPhoneNumber = dto.MainPhoneNumber;

        await _context.SaveChangesAsync();

        await _publishEndpoint.Publish(new UpdateUserEvent
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            MainPhoneNumber = user.MainPhoneNumber
        });

        string? imageUrl = dto.Image != null ? $"{_currentUser.BaseUrl}/userFiles/{user.Image}" : null;
        
        return imageUrl;
    }

    public async Task CheckUserExistAsync(string email, string phoneNumber)
    {
        var user = await _context.Users.Where(x => (x.Email == email || x.MainPhoneNumber == phoneNumber) && x.Id != _currentUser.UserGuid)
            .Select(x => new
            {
                x.Email,
                x.MainPhoneNumber
            })
            .FirstOrDefaultAsync();

        if (user != null)
        {
            if (user.Email == email)
                throw new BadRequestException("USEREXISTEXCEPTION_EMAIL");
            else if (user.MainPhoneNumber == phoneNumber)
                throw new BadRequestException("USEREXISTEXCEPTION_PHONE");
        }
    }
}