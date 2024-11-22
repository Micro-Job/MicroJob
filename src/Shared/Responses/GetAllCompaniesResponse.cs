using SharedLibrary.Dtos.CompanyDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Responses
{
    public class GetAllCompaniesResponse
    {
        public ICollection<CompanyDto> Companies { get; set; }
    }
}
