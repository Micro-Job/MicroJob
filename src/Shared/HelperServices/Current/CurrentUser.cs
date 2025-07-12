using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Enums;
using System.Globalization;
using System.Security.Claims;

namespace SharedLibrary.HelperServices.Current
{
    public class CurrentUser(IHttpContextAccessor _contextAccessor, IConfiguration _configuration) : ICurrentUser
    {
        public string? UserId => _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Sid)?.Value;
        public Guid? UserGuid => UserId != null ? Guid.Parse(UserId) : null;
        public string? UserFullName => _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        public string BaseUrl => _configuration["ApiGateway:BaseUrl"]!;

        public bool IsAuthenticated => _contextAccessor.HttpContext.User.Identity.IsAuthenticated;

        public byte UserRole
        {
            get
            {
                var roleClaim = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

                if (Enum.TryParse(roleClaim, out UserRole role))
                {
                    return (byte)role;
                }

                return 0;

            }
        }

        public LanguageCode LanguageCode
        {
            get
            {
                var currentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();

                if (!Enum.TryParse<LanguageCode>(currentLanguage, true, out var languageEnum))
                {
                    languageEnum = LanguageCode.en;
                }

                return languageEnum;
            }
        }

    }
}
