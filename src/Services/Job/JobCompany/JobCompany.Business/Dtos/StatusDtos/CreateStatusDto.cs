using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.StatusDtos
{
    public record CreateStatusDto
    {
        public string StatusName { get; set; }
    }

    public class CreateStatusValidator : AbstractValidator<CreateStatusDto>
    {
        public CreateStatusValidator()
        {
            RuleFor(x => x.StatusName.Trim())
                .NotEmpty()
                    .WithMessage("Status adı boş ola bilməz!")
                .Length(2, 32)
                    .WithMessage("Adın uzunluğu 2-32 arasında olmalıdır.");
        }
    }
}
