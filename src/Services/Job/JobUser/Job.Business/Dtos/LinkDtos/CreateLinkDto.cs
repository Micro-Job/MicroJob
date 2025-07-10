using FluentValidation;
using Job.Core.Enums;
using SharedLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Business.Dtos.LinkDtos
{
    public class CreateLinkDto
    {
        public LinkEnum LinkType { get; set; }
        public string Url { get; set; }
    }

    public class CreateLinkValidator : AbstractValidator<CreateLinkDto>
    {
        public CreateLinkValidator()
        {
            RuleFor(x => x.Url).MaximumLength(100).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"));
        }
    }
}
