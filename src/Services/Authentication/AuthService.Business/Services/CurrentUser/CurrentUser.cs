using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AuthService.Business.Services.CurrentUser
{
    public class CurrentUser(IHttpContextAccessor _contextAccessor) : ICurrentUser
    {
        public string UserId => _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value;
        public string? UserName => _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        public string? BaseUrl => $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host.Value}{_contextAccessor.HttpContext.Request.PathBase.Value}";
    }
}