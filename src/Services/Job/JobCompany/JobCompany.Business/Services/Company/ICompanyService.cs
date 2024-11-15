using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Business.Dtos;

namespace JobCompany.Business.Services.Company
{
    public interface ICompanyService
    {
        Task UpdateCompanyAsync(Dtos.CompanyDtos.CompanyUpdateDto dto);
    }
}