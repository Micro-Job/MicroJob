using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public record ApplicationRemoveDto
    {
        public string ApplicationId { get; set; }
    }

    public class ApplicationRemoveDtoValidator : AbstractValidator<ApplicationRemoveDto>
    {
        public ApplicationRemoveDtoValidator()
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty()
                .WithMessage("ApplicationId is required.");
        }
    }
}