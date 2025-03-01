﻿using System.Security.Claims;
using AuthService.Business.Dtos;
using AuthService.Business.Exceptions.UserException;
using AuthService.Business.HelperServices.TokenHandler;
using AuthService.Business.Publishers;
using AuthService.Core.Entities;
using AuthService.Core.Enums;
using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.EmailDtos;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace AuthService.Business.Services.Auth
{
    public class AuthService(
        AppDbContext _context,
        ITokenHandler _tokenHandler,
        IHttpContextAccessor _httpContext,
        IPublishEndpoint _publishEndpoint,
        EmailPublisher _publisher,
        IFileService _fileService
    ) : IAuthService
    {
        private string _ipAddress = _httpContext.HttpContext.Connection.RemoteIpAddress?.ToString();
        private string _userId = _httpContext.HttpContext?.User?.FindFirst(ClaimTypes.Sid)?.Value;

        public async Task RegisterAsync(RegisterDto dto)
        {
            var userCheck = await _context.Users.FirstOrDefaultAsync(x =>
                x.Email == dto.Email || x.MainPhoneNumber == dto.MainPhoneNumber
            );

            if (userCheck != null)
                throw new UserExistException();
            if (dto.Password != dto.ConfirmPassword)
                throw new WrongPasswordException();

            FileDto fileResult =
                dto.Image != null
                    ? await _fileService.UploadAsync(FilePaths.image, dto.Image)
                    : new FileDto();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MainPhoneNumber = dto.MainPhoneNumber,
                RegistrationDate = DateTime.Now,
                Password = _tokenHandler.GeneratePasswordHash(dto.Password),
                Image = dto.Image != null ? $"{fileResult.FilePath}/{fileResult.FileName}" : null,
                UserRole =
                    dto.UserStatus == 1 ? UserRole.SimpleUser
                    : dto.UserStatus == 2 ? UserRole.EmployeeUser
                    : throw new InvalidUserStatusException(),
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new UserRegisteredEvent { UserId = user.Id });

            await _publisher.SendEmail(
                new EmailMessage
                {
                    Email = dto.Email,
                    Subject = "Xoş gəldiniz",
                    Content = "Qeydiyyat uğurla başa çatdı",
                }
            );

            // sifre yaratmaq ucun mail gondermek
            //await _emailService.SendSetPassword(dto.Email, await GeneratePasswordResetTokenAsync(user));
        }

        public async Task CompanyRegisterAsync(RegisterCompanyDto dto)
        {
            if (!dto.Policy)
                throw new PolicyException();

            var userCheck = await _context.Users.FirstOrDefaultAsync(x =>
                x.Email == dto.Email || x.MainPhoneNumber == dto.MainPhoneNumber
            );

            // email veya istifadeci adı tekrarlanmasını yoxla
            if (userCheck != null)
                throw new UserExistException();
            if (dto.Password != dto.ConfirmPassword)
                throw new WrongPasswordException();

            FileDto fileResult =
                dto.Image != null
                    ? await _fileService.UploadAsync(FilePaths.image, dto.Image)
                    : new FileDto { FilePath = "Files/Images", FileName = "defaultlogo.jpg" };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MainPhoneNumber = dto.MainPhoneNumber,
                RegistrationDate = DateTime.Now,
                Password = _tokenHandler.GeneratePasswordHash(dto.Password),
                Image = fileResult.FilePath + "/" + fileResult.FileName,
                UserRole = UserRole.CompanyUser,
            };


            //var company = new Company { Id = Guid.NewGuid(), UserId = user.Id };
            //await _context.Companies.AddAsync(company);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(
                new CompanyRegisteredEvent
                {
                    CompanyId = Guid.NewGuid(),
                    UserId = user.Id,
                    CompanyName = dto.CompanyName.Trim(),
                    CompanyLogo = user.Image,
                }
            );

            await _publishEndpoint.Publish(new UserRegisteredEvent { UserId = user.Id });

            await _publisher.SendEmail(
                new EmailMessage
                {
                    Email = dto.Email,
                    Subject = "Xoş gəldiniz",
                    Content = "Qeydiyyat uğurla başa çatdı",
                }
            );
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            // useri email ve ya userName ile tapmaq
            var user = await _context
                .Users.Include(u => u.LoginLogs)
                .FirstOrDefaultAsync(x => x.Email == dto.UserNameOrEmail);
            if (user == null)
                throw new LoginFailedException();

            // hesabin hal hazirda bloklanmadigini yoxla
            if (user.LockDownDate.HasValue && user.LockDownDate.Value > DateTime.Now)
                throw new AccountLockedException(user.LockDownDate.Value);

            var hashedPassword = _tokenHandler.GeneratePasswordHash(dto.Password);
            if (user.Password != hashedPassword)
            {
                throw new LoginFailedException();
                #region LoginLog
                //await _context.LoginLogs.AddAsync(
                //    new LoginLog
                //    {
                //        UserId = user.Id,
                //        Date = DateTime.Now,
                //        IsSucceed = false,
                //        IP = _ipAddress,
                //    }
                //);

                //// sonuncu ugurlu login saatini al
                //var lastSuccessfulLogin =
                //    user.LoginLogs.Where(l => l.IsSucceed)
                //        .OrderByDescending(l => l.Date)
                //        .FirstOrDefault()
                //        ?.Date ?? DateTime.MinValue;

                //// user bloklanma vaxti kecibse failedAttempts ucun lockDownDate-e gore hesablama apar
                //if (user.LockDownDate.HasValue && user.LockDownDate.Value < DateTime.Now)
                //    lastSuccessfulLogin = user.LockDownDate.Value;

                //// sonuncu ugurlu login cehdinden sonraki ugursuz cehdlerin sayini al
                //var failedAttempts = user
                //    .LoginLogs.Where(l => !l.IsSucceed && l.Date > lastSuccessfulLogin)
                //    .Count();

                //if (failedAttempts >= 3)
                //{
                //    user.LockDownDate = DateTime.Now.AddHours(1);
                //}

                //await _context.SaveChangesAsync();
                #endregion
            }

            //await _context.LoginLogs.AddAsync(
            //    new LoginLog
            //    {
            //        UserId = user.Id,
            //        Date = DateTime.Now,
            //        IsSucceed = true,
            //        IP = _ipAddress,
            //    }
            //);
            // ugurlu loginden sonra bloklanma vaxtini sifirla
            //user.LockDownDate = null;
            //await _context.SaveChangesAsync();

            var accessToken = _tokenHandler.CreateToken(user, 60);
            var refreshToken = _tokenHandler.GenerateRefreshToken(accessToken, 1440);

            user.RefreshToken = refreshToken.Token;
            user.RefreshTokenExpireDate = refreshToken.Expires;
            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                UserId = user.Id.ToString(),
                FullName = user.FirstName + " " + user.LastName,
                UserStatusId = (byte)user.UserRole,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Expires = refreshToken.Expires,
            };
        }

        public async Task<TokenResponseDto> LoginWithRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new BadRequestException("Refresh token tələb olunur.");

            var user = await _context.Users.FirstOrDefaultAsync(x =>
                x.RefreshToken == refreshToken
            );

            if (user == null)
                throw new LoginFailedException("Keçərsiz refresh token.");

            if (user.RefreshTokenExpireDate < DateTime.Now)
                throw new RefreshTokenExpiredException();

            var newToken = _tokenHandler.CreateToken(user, 60);
            var newRefreshToken = _tokenHandler.GenerateRefreshToken(newToken, 1440);

            user.RefreshToken = newRefreshToken.Token;
            user.RefreshTokenExpireDate = newRefreshToken.Expires;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                UserId = user.Id.ToString(),
                FullName = user.FirstName + " " + user.LastName,
                AccessToken = newToken,
                RefreshToken = newRefreshToken.Token,
                UserStatusId = (byte)user.UserRole,
                Expires = newRefreshToken.Expires,
            };
        }

        /// <summary>
        /// mail-e sifre sifirlama sorgusu gonderir
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        public async Task ResetPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
                throw new NotFoundException<User>("İstiadəçi mövcud deyil.");

            var token = _tokenHandler.CreatePasswordResetToken(user);

            var passwordToken = new PasswordToken
            {
                Token = token,
                UserId = user.Id,
                ExpireTime = DateTime.Now.AddHours(1),
                //ExpireTime = DateTime.Now.AddMinutes(3),
            };

            await _context.PasswordTokens.AddAsync(passwordToken);
            await _context.SaveChangesAsync();

            //Console.WriteLine($" Email : {email} /n UserName : {user.UserName} /n Token : {token}");
            //await _emailService.SendChangePassword(email, user.UserName, token);
        }

        /// <summary>
        /// userin yeni şifresini mueyyen etmek ucun kecici tokeni tesdiqleyir ve sifreni update edir
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        /// <exception cref="BadRequestException"></exception>
        public async Task ConfirmPasswordResetAsync(PasswordResetDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                throw new NotFoundException<User>("İstiadəçi mövcud deyil.");

            var passwordToken = await _context.PasswordTokens.FirstOrDefaultAsync(pt =>
                pt.Token == dto.Token && pt.UserId == user.Id && pt.ExpireTime > DateTime.Now
            );

            if (passwordToken == null)
                throw new BadRequestException();

            user.Password = _tokenHandler.GeneratePasswordHash(dto.NewPassword);

            _context.PasswordTokens.Remove(passwordToken);
            await _context.SaveChangesAsync();
        }
    }
}
