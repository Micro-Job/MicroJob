using FluentValidation;
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? CompanyName { get; set; }
        public string MainPhoneNumber { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool Policy { get; set; }
        public bool IsCompany { get; set; }
    }

    public class RegisterCompanyValidator : AbstractValidator<RegisterCompanyDto>
    {
        public RegisterCompanyValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().When(x=> x.IsCompany)
                .WithMessage("Xublar xanim deyir ki bos ola bilmez");
        }
    }
}
