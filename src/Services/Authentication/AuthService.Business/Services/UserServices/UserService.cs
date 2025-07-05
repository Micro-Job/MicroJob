using AuthService.Business.Dtos;
using AuthService.Business.Exceptions.UserException;
using AuthService.Business.Services.Auth;
using AuthService.Core.Entities;
using AuthService.DAL.Contexts;
using Azure;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using SharedLibrary.Statics;

namespace AuthService.Business.Services.UserServices
{
    public class UserService(AppDbContext _context, AuthenticationService _authService, IRequestClient<GetCompaniesDataByUserIdsRequest> _companyDataRequest)
    {
        /// <summary> Admin paneldə bütün istifadəçilər siyahısının göründüyü hissə </summary>  
        //TODO : Bu endpoint optimize edilmelidir
        public async Task<DataListDto<BasicUserInfoDto>> GetAllUsersAsync(UserRole userRole, string? fullName, string? email, string? phoneNumber, int pageIndex = 1, int pageSize = 10)
        {
            var userQuery = _context.Users.Where(u => u.UserRole == userRole)
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Email,
                    x.MainPhoneNumber
                })
                .AsQueryable()
                .AsNoTracking();

            if (!string.IsNullOrEmpty(email))
            {
                email = email.Trim().ToLower();
                userQuery = userQuery.Where(u => u.Email.Contains(email));
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                phoneNumber = phoneNumber.Trim();
                userQuery = userQuery.Where(u => u.MainPhoneNumber.Contains(phoneNumber));
            }

            List<Guid> filteredUserIds = [];
            Dictionary<Guid, CompanyNameAndImageDto> companyDataByUserId = [];

            // CompanyUser-dirsə şirkət adını gətirmək üçün jobcompany-yə sorğu atır
            if (userRole == UserRole.CompanyUser)
            {
                var allUserIds = await userQuery.Select(u => u.Id).ToListAsync();

                var companyResponse = await _companyDataRequest.GetResponse<GetCompaniesDataByUserIdsResponse>(
                    new GetCompaniesDataByUserIdsRequest
                    {
                        UserIds = allUserIds,
                        CompanyName = fullName
                    });

                companyDataByUserId = companyResponse.Message.Companies;
                filteredUserIds = companyDataByUserId.Keys.ToList();

                if (filteredUserIds.Count == 0)
                {
                    return new DataListDto<BasicUserInfoDto>
                    {
                        Datas = [],
                        TotalCount = 0
                    };
                }

                userQuery = userQuery.Where(u => filteredUserIds.Contains(u.Id));
            }
            else if ((userRole == UserRole.SimpleUser || userRole == UserRole.EmployeeUser) && !string.IsNullOrWhiteSpace(fullName))
            {
                // SimpleUser üçün searchTerm varsa, fullname ilə axtarış edir
                userQuery = userQuery
                    .Where(u => (u.FirstName + u.LastName).Contains(fullName));
            }

            var users = await userQuery
                .Select(u => new BasicUserInfoDto
                {
                    UserId = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    MainPhoneNumber = u.MainPhoneNumber
                })
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int totalCount = await userQuery.CountAsync();

            // CompanyUser üçün CompanyName əvəzlənməsi
            if (companyDataByUserId.Count != 0)
            {
                foreach (var user in users)
                {
                    if (companyDataByUserId.TryGetValue(user.UserId, out var companyInfo))
                    {
                        user.FullName = companyInfo.CompanyName ?? user.FullName;
                    }
                }
            }

            return new DataListDto<BasicUserInfoDto>
            {
                Datas = users,
                TotalCount = totalCount
            };
        }


        #region Operatorlar

        /// <summary> Admin paneldə bütün operatorlar siyahısının göründüyü hissə </summary>
        public async Task<DataListDto<OperatorInfoDto>> GetAllOperatorsAsync(UserRole? userRole, string? fullName, string? email, string? phoneNumber, int pageIndex = 1, int pageSize = 10)
        {
            var operatorsQuery = _context.Users
            .AsNoTracking()
            .Where(u => userRole == null
                ? (u.UserRole == UserRole.Operator || u.UserRole == UserRole.ChiefOperator)
                : u.UserRole == userRole);

            if (userRole != null)
                operatorsQuery = operatorsQuery.Where(u => u.UserRole == userRole);

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                fullName = fullName.Trim().ToLower();
                operatorsQuery = operatorsQuery.Where(u => (u.FirstName + " " + u.LastName).ToLower().Contains(fullName));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                email = email.Trim().ToLower();
                operatorsQuery = operatorsQuery.Where(o => o.Email.Contains(email));
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                phoneNumber = phoneNumber.Trim();
                operatorsQuery = operatorsQuery.Where(o => o.MainPhoneNumber.Contains(phoneNumber));
            }

            var totalCount = await operatorsQuery.CountAsync();

            var users = await operatorsQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new OperatorInfoDto
                {
                    UserId = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    MainPhoneNumber = u.MainPhoneNumber,
                    UserRole = u.UserRole,
                }).ToListAsync();

            return new DataListDto<OperatorInfoDto>
            {
                Datas = users,
                TotalCount = totalCount
            };
        }

        public async Task<OperatorInfoDto> GetOperatorByIdAsync(string id)
        {
            var userId = Guid.Parse(id);

            var operatorInfo = await _context.Users
                .AsNoTracking()
                .Where(u => (u.UserRole == UserRole.Operator || u.UserRole == UserRole.ChiefOperator) && u.Id == userId)
                .Select(u => new OperatorInfoDto
                {
                    UserId = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    MainPhoneNumber = u.MainPhoneNumber,
                    UserRole = u.UserRole,
                }).FirstOrDefaultAsync()
                ?? throw new UserNotFoundException();

            return operatorInfo;
        }

        public async Task AddOperatorAsync(OperatorAddDto dto)
        {
            bool isExist = await _context.Users.AnyAsync(u => u.Email == dto.Email.Trim());

            if (isExist) throw new UserExistException(MessageHelper.GetMessage("USEREXISTEXCEPTION_EMAIL"));

            isExist = await _context.Users.AnyAsync(u => u.MainPhoneNumber == dto.MainPhoneNumber.Trim());

            if (isExist) throw new UserExistException(MessageHelper.GetMessage("USEREXISTEXCEPTION_PHONE"));

            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim(),
                MainPhoneNumber = dto.MainPhoneNumber.Trim(),
                UserRole = dto.UserRole,
                RegistrationDate = DateTime.Now,
                Password = userId.ToString(),
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await _authService.ResetPasswordAsync(user.Email);
        }

        public async Task UpdateOperatorAsync(OperatorUpdateDto dto)
        {
            var user = await _context.Users
                .Where(u => (u.UserRole == UserRole.Operator || u.UserRole == UserRole.ChiefOperator) && u.Id == dto.UserId)
                .FirstOrDefaultAsync()
                ?? throw new UserNotFoundException();

            if (user.Email != dto.Email.Trim())
            {
                bool isExist = await _context.Users.AnyAsync(u => u.Email == dto.Email.Trim());

                if (isExist) throw new UserExistException(MessageHelper.GetMessage("USEREXISTEXCEPTION_EMAIL"));
            }

            if (user.MainPhoneNumber != dto.MainPhoneNumber.Trim())
            {
                bool isExist = await _context.Users.AnyAsync(u => u.MainPhoneNumber == dto.MainPhoneNumber.Trim());

                if (isExist) throw new UserExistException(MessageHelper.GetMessage("USEREXISTEXCEPTION_PHONE"));
            }

            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.Email = dto.Email.Trim();
            user.MainPhoneNumber = dto.MainPhoneNumber.Trim();
            user.UserRole = dto.UserRole;

            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
