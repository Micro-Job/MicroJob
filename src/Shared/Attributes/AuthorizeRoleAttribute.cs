using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SharedLibrary.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeRoleAttribute(params UserRole[] roles) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(meta => meta is AllowAnonymousAttribute);

        if (hasAllowAnonymous) return;  

        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userRoleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
        
        UserRole enumUserRoleClaim = Enum.Parse<UserRole>(userRoleClaim!);

        if (userRoleClaim == null || !roles.Contains(enumUserRoleClaim))
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
