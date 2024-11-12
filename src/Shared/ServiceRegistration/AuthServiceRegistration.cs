using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.ExternalServices.FileService;
using System.Text;

namespace SharedLibrary.ServiceRegistration
{
    public static class AuthServiceRegistration
    {
        public static void AddAuth(this IServiceCollection services, string issuer, string audience, string key)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    LifetimeValidator = (_, expires, token, _) => token != null ? DateTime.Now < expires : false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
            services.AddAuthorization();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IFileService, FileService>();
        }
        //builder.Services.AddAuth(builder.Configuration["Jwt:Issuer"], builder.Configuration["Jwt:Audience"], builder.Configuration["Jwt:SigningKey"]);
    }
}
