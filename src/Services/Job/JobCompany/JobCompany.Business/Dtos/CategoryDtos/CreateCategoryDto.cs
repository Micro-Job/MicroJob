using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace JobCompany.Business.Dtos.CategoryDtos
{
    public record CreateCategoryDto
    {
        public string CategoryName { get; set; }
    }

    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
            .MinimumLength(2) .WithMessage("Category name length must be greater than 2")
            .NotEmpty() .WithMessage("Category name is required");
        }
    }
}