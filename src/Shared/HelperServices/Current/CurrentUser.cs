using Microsoft.AspNetCore.Http;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.HelperServices.Current
{
    public class CurrentUser(IHttpContextAccessor _contextAccessor) : ICurrentUser
    {
        public string? UserId => _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Sid)?.Value;
        public Guid? UserGuid => UserId != null ? Guid.Parse(UserId) : null;
        public string? UserFullName => _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        public string? BaseUrl =>
            $"{_contextAccessor.HttpContext?.Request.Scheme}://{_contextAccessor.HttpContext?.Request.Host.Value}{_contextAccessor.HttpContext?.Request.PathBase.Value}";

        public byte UserRole => Convert.ToByte(_contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value);

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
