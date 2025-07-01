using AuthService.Business.Dtos;
using AuthService.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Business.HelperServices.TokenHandler;

public class TokenHandler(IConfiguration _configuration)
{
    public string CreateToken(User user, int expires = 60)
    {
        List<Claim> claims =
        [
           new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
           new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
           new Claim(ClaimTypes.Sid, user.Id.ToString()),
           new Claim(ClaimTypes.Role, user.UserRole.ToString()),
        ];

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]!));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken jwtSecurity = new(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            DateTime.Now,
            DateTime.Now.AddMinutes(20000),
            //DateTime.Now.AddMinutes(expires),
            credentials);
        JwtSecurityTokenHandler jwtHandler = new();
        string token = jwtHandler.WriteToken(jwtSecurity);
        return token;
    }

    public string CreatePasswordResetToken(User user)
    {
        List<Claim> claims =
        [
           new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
           new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
           new Claim(ClaimTypes.Sid, user.Id.ToString()),
           new Claim(ClaimTypes.Email, user.Email),
        ];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            DateTime.Now,
            DateTime.Now.AddHours(1), 
            credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GeneratePasswordHash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes("siesco" + input + "nantech");
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            var enc = Encoding.GetEncoding(0);
            byte[] buffer = enc.GetBytes(Convert.ToHexString(hashBytes));
            var sha1 = SHA1.Create();
            var hash = BitConverter.ToString(sha1.ComputeHash(buffer)).Replace("-", "");
            return hash;
        }
    }

    public RefreshToken GenerateRefreshToken(string token, int min)
    {
        var refreshToken = new RefreshToken
        {
            Token = GeneratePasswordHash(token),
            Created = DateTime.Now,
            Expires = DateTime.Now.AddMinutes(min),
        };

        return refreshToken;
    }

}
