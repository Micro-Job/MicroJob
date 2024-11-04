using AuthService.Business.Dtos;
using AuthService.Business.Exceptions.UserException;
using AuthService.DAL.Contexts;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Services.UserServices
{
    public class UserService(AppDbContext _context) : IUserService
    {
        public async Task<UserInformationDto> GetUserInformationAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Id == userGuid).Select(x=> new UserInformationDto
            {
                UserId = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                MainPhoneNumber = x.MainPhoneNumber,
                Email = x.Email
            }) ?? throw new UserNotFoundException();

            return user;
        }
    }
}
