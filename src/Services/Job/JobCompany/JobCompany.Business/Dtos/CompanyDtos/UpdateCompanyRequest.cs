using FluentValidation;
using JobCompany.Business.Dtos.NumberDtos;

namespace JobCompany.Business.Dtos.CompanyDtos;

public record UpdateCompanyRequest
{
    public CompanyUpdateDto? CompanyDto { get; set; }
    public ICollection<UpdateNumberDto>? NumbersDto { get; set; }
}

public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator()
    {
        When(x => x.CompanyDto != null, () =>
        {
            RuleFor(x => x.CompanyDto!)    
                .SetValidator(new CompanyUpdateDtoValidator());
        });
    }
}