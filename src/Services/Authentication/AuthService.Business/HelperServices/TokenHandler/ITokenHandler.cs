using AuthService.Business.Dtos;
using AuthService.Core.Entities;

namespace AuthService.Business.HelperServices.TokenHandler
{
    public interface ITokenHandler
    {
        RefreshToken GenerateRefreshToken(string token, int min);
        string CreatePasswordResetToken(User user);
        string GeneratePasswordHash(string input);
        string CreateToken(User user, int expires = 60);
    }
}
