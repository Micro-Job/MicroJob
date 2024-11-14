using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Dtos
{
    public class RegisterCompanyDto
    {
        public string CompanyName { get; set; }
        public string MainPhoneNumber { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public IFormFile? Image { get; set; }
    }
}
