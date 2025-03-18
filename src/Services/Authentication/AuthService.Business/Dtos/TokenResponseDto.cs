using AuthService.Core.Entities;
using AuthService.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Dtos
{
    public class TokenResponseDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public byte UserStatusId { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public string RefreshToken { get; set; }
        public string? UserImage { get; set; }
        //public byte UserRole { get; set; }
    }
}
