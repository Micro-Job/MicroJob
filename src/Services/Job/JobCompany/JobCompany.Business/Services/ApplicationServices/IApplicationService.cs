using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.ApplicationDtos;

namespace JobCompany.Business.Services.ApplicationServices
{
    public interface IApplicationService
    {
        Task CreateApplicationAsync(ApplicationCreateDto dto);
    }
}
