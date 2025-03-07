using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Business.Services.CurrentUser
{
    public interface ICurrentUser
    {
        public string? UserId { get; }
        public Guid? UserGuid { get; }
        public string? UserName { get; }
        public string? BaseUrl { get; }
    }
}