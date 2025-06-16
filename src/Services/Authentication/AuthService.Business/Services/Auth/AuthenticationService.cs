using AuthService.Business.Dtos;
using AuthService.Business.Exceptions.UserException;
using AuthService.Business.HelperServices.Email;
using AuthService.Business.HelperServices.TokenHandler;
using AuthService.Core.Entities;
using AuthService.DAL.Contexts;
using Azure;
using HtmlAgilityPack;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Dtos.EmailDtos;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.Business.Services.Auth
{
    public class AuthenticationService(AppDbContext _context, ITokenHandler _tokenHandler, IPublishEndpoint _publishEndpoint, IConfiguration _configuration, IEmailService _emailService, ICurrentUser _currentUser, IRequestClient<GetCompaniesDataByUserIdsRequest> _companyDataClient , IRequestClient<CheckVoenRequest> _voenCheckRequest) 
    {
        public async Task RegisterAsync(RegisterDto dto)
        {
            dto.MainPhoneNumber = dto.MainPhoneNumber.Trim();
            dto.Email = dto.Email.Trim();
            await CheckUserExistAsync(dto.Email, dto.MainPhoneNumber);

            CheckPasswordAndThrowException(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                MainPhoneNumber = dto.MainPhoneNumber,
                RegistrationDate = DateTime.Now,
                JobStatus = JobStatus.ActivelySeekingJob,
                Password = _tokenHandler.GeneratePasswordHash(dto.Password),
                UserRole = UserRole.SimpleUser
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new UserRegisteredEvent
            {
                UserId = user.Id,
                JobStatus = user.JobStatus,
                FirstName = user.FirstName,
                LastName = user.LastName,
            });

            await CreateBalance(user.Id, user.FirstName, user.LastName);

            await _emailService.SendRegister(user.Email, $"{user.FirstName} {user.LastName}");
        }

        public async Task CompanyRegisterAsync(RegisterCompanyDto dto)
        {
            if (!dto.Policy)
                throw new PolicyException();

            CheckPasswordAndThrowException(dto.Password);
            
            // email veya istifadeci adı tekrarlanmasını yoxla
            dto.MainPhoneNumber = dto.MainPhoneNumber.Trim();
            dto.Email = dto.Email.Trim();
            await CheckUserExistAsync(dto.Email, dto.MainPhoneNumber);

            //TODO : exceptionlar deyismelidir
            if (dto.IsCompany && dto.VOEN != null)
            {
                string? companyName = await GetCompanyNameByVOENAsync(dto.VOEN);
                if(string.IsNullOrEmpty(companyName))
                    throw new NotFoundException<User>("VÖEN tapılmadı.");

                var voenResponse = await _voenCheckRequest.GetResponse<CheckVoenResponse>(new CheckVoenRequest
                {
                    VOEN = dto.VOEN
                });

                if (voenResponse.Message.IsExist)
                    throw new UserExistException("VÖEN istifadə edilib.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MainPhoneNumber = dto.MainPhoneNumber,
                RegistrationDate = DateTime.Now,
                Password = _tokenHandler.GeneratePasswordHash(dto.Password),
                UserRole = dto.IsCompany ? UserRole.CompanyUser : UserRole.EmployeeUser
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(
                new CompanyRegisteredEvent
                {
                    CompanyId = Guid.NewGuid(),
                    UserId = user.Id,
                    CompanyName = dto.IsCompany ? dto.CompanyName.Trim() : null,
                    IsCompany = dto.IsCompany,
                    VOEN = dto.VOEN
                }
            );

            //await _publishEndpoint.Publish(new UserRegisteredEvent
            //{
            //    UserId = user.Id,
            //    FirstName = user.FirstName,
            //    LastName = user.LastName
            //});

            await CreateBalance(user.Id, user.FirstName, user.LastName);

            await _emailService.SendRegister(dto.Email, $"{user.FirstName} {user.LastName}");
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            // useri email ve ya userName ile tapmaq
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.UserNameOrEmail)
                ?? throw new LoginFailedException();

            var hashedPassword = _tokenHandler.GeneratePasswordHash(dto.Password);
            if (user.Password != hashedPassword)
            {
                throw new LoginFailedException();
            }

            var accessToken = _tokenHandler.CreateToken(user, 60);
            var refreshToken = _tokenHandler.GenerateRefreshToken(accessToken, 1440);

            user.RefreshToken = refreshToken.Token;
            user.RefreshTokenExpireDate = refreshToken.Expires;
            await _context.SaveChangesAsync();

            var responseDto = new TokenResponseDto
            {
                UserId = user.Id.ToString(),
                FullName = $"{user.FirstName} {user.LastName}",
                UserStatusId = (byte)user.UserRole,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Expires = refreshToken.Expires,
                UserImage = user.Image != null ? $"{_currentUser.BaseUrl}/{user.Image}" : null
            };

            if (user.UserRole == UserRole.CompanyUser || user.UserRole == UserRole.EmployeeUser)
            {
                var companyResponse = await _companyDataClient.GetResponse<GetCompaniesDataByUserIdsResponse>(
                    new GetCompaniesDataByUserIdsRequest
                    {
                        UserIds = [user.Id]
                    });

                var companyData = companyResponse.Message.Companies[user.Id];

                if (user.UserRole == UserRole.CompanyUser)
                    responseDto.FullName = companyData.CompanyName;

                responseDto.UserImage = $"{_configuration["JobCompany:BaseUrl"]}/{companyData.CompanyLogo}" ?? null;
            }

            return responseDto;
        }

        public async Task<TokenResponseDto> LoginWithRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new BadRequestException(MessageHelper.GetMessage("SESSION_EXPIRED"));

            var user = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken) 
                ?? throw new LoginFailedException();

            if (user.RefreshTokenExpireDate < DateTime.Now)
                throw new RefreshTokenExpiredException();

            var newToken = _tokenHandler.CreateToken(user, 60);
            var newRefreshToken = _tokenHandler.GenerateRefreshToken(newToken, 1440);

            user.RefreshToken = newRefreshToken.Token;
            user.RefreshTokenExpireDate = newRefreshToken.Expires;
            
            await _context.SaveChangesAsync();

            var responseDto = new TokenResponseDto
            {
                UserId = user.Id.ToString(),
                FullName = $"{user.FirstName} {user.LastName}",
                AccessToken = newToken,
                RefreshToken = newRefreshToken.Token,
                UserStatusId = (byte)user.UserRole,
                Expires = newRefreshToken.Expires,
                UserImage = user.Image != null ? $"{_currentUser.BaseUrl}/{user.Image}" : null
            };

            if (user.UserRole == UserRole.CompanyUser || user.UserRole == UserRole.EmployeeUser)
            {
                var companyResponse = await _companyDataClient.GetResponse<GetCompaniesDataByUserIdsResponse>(
                    new GetCompaniesDataByUserIdsRequest
                    {
                        UserIds = [user.Id]
                    });

                var companyData = companyResponse.Message.Companies[user.Id];

                responseDto.FullName = companyData.CompanyName;
                responseDto.UserImage = $"{_configuration["JobCompany:BaseUrl"]}/{companyData.CompanyLogo}" ?? null;
            }

            return responseDto;
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

            if (user == null) return;

            var existedPasswordToken = await _context.PasswordTokens.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (existedPasswordToken != null)
            {
                _context.PasswordTokens.Remove(existedPasswordToken);
                await _context.SaveChangesAsync();
            }

            var token = _tokenHandler.CreatePasswordResetToken(user);

            var passwordToken = new PasswordToken
            {
                Token = token,
                UserId = user.Id,
                ExpireTime = DateTime.Now.AddHours(1),
            };

            await _context.PasswordTokens.AddAsync(passwordToken);
            await _context.SaveChangesAsync();
            await _emailService.SendResetPassword(user, token);

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
            CheckPasswordAndThrowException(dto.NewPassword);

            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(dto.Token);

            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException();

            var passwordToken = await _context.PasswordTokens.FirstOrDefaultAsync(pt =>
                pt.Token == dto.Token && pt.UserId == user.Id && pt.ExpireTime > DateTime.Now
            ) ?? throw new BadRequestException();

            user.Password = _tokenHandler.GeneratePasswordHash(dto.NewPassword);

            _context.PasswordTokens.Remove(passwordToken);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// User-in şifrəsini yeniləyir
        /// </summary>
        public async Task UpdatePasswordAsync(string oldPassword, string newPassword)
        {
            CheckPasswordAndThrowException(newPassword);

            var hashOldPassword = _tokenHandler.GeneratePasswordHash(oldPassword);

            var user = await _context.Users.Where(ap => ap.Id == _currentUser.UserGuid).FirstOrDefaultAsync()
                ?? throw new NotFoundException<User>();

            if (user.Password != hashOldPassword)
                throw new OldPasswordWrongException();

            user.Password = _tokenHandler.GeneratePasswordHash(newPassword);

            await _context.SaveChangesAsync();
        }

        //TODO : burada yeni auth proyektinde user tableinda voen tutulmalidir mi 
        public async Task CheckUserExistAsync(string email, string phoneNumber)
        {
            var user = await _context.Users.Where(x => x.Email == email || x.MainPhoneNumber == phoneNumber)
                .Select(x => new
                {
                    x.Email,
                    x.MainPhoneNumber
                })
                .FirstOrDefaultAsync();

            if (user != null)
            {
                if (user.Email == email)
                    throw new ExistEmailException();
                else if (user.MainPhoneNumber == phoneNumber)
                    throw new ExistPhoneNumberException();
            }
        }

        private async Task CreateBalance(Guid userId, string firstName, string lastName)
        {
            await _publishEndpoint.Publish(new CreateBalanceEvent
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName
            });
        }

        public async Task<string?> GetCompanyNameByVOENAsync(string voen)
        {
            using (HttpClient client = new HttpClient())
            {
                byte checkCount = 0;
                client.Timeout = TimeSpan.FromSeconds(0.6);
                while (checkCount < 5)
                {
                    try
                    {
                        string htmlContent = await client.GetStringAsync("https://www.e-taxes.gov.az/ebyn/checkCerts.jsp?name=" + voen + "&submit");
                        HtmlDocument htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(htmlContent);
                        string titleContent = htmlDocument.DocumentNode.SelectSingleNode("//td[contains(@width, '69')]")?.InnerHtml;
                        if (!string.IsNullOrEmpty(titleContent)) return titleContent;
                        return null;
                    }
                    catch (Exception)
                    {
                        checkCount++;
                    }
                }
            }
            return null;
        }

        private void CheckPasswordAndThrowException(string password)
        {
            if (!(password.Length >= 8 &&
                password.Length <= 64 &&
                password.Any(char.IsUpper) &&
                password.Any(char.IsNumber) &&
                password.Any(char.IsPunctuation)))
                throw new BadRequestException(MessageHelper.GetMessage("PASSWORD_IS_NOT_VALID"));
        }
    }
}
