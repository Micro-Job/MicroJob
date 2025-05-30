﻿using FluentValidation;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.NumberDtos
{
    public class UpdateNumberDto
    {
        public string? Id { get; set; } 
        public string PhoneNumber { get; set; }
    }

    public class UpdateNumberValidator : AbstractValidator<UpdateNumberDto>
    {
        public UpdateNumberValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(?:\+\d{1,3})?\d{1,4}\d{7,10}$")
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
        }
    }
}