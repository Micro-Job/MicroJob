using Microsoft.AspNetCore.Http;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Middlewares
{
    public class LanguageMiddleware(RequestDelegate _next)
    {
        public async Task Invoke(HttpContext httpContext)
        {
            string? language = httpContext.Request.Cookies["Language"];
            if (string.IsNullOrEmpty(language))
            {
                language = httpContext.Request.Headers["Accept-Language"].FirstOrDefault()?.Split(",")[0];
            }
            if (string.IsNullOrEmpty(language))
            {
                language = LanguageCode.en.ToString();
            }

            var culture = new CultureInfo(language);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await _next(httpContext);
        }
    }
}
