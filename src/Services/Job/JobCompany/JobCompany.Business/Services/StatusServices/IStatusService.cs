using JobCompany.Business.Dtos.StatusDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Services.StatusServices
{
    public interface IStatusService
    {
        Task CreateStatusAsync(CreateStatusDto dto);

        Task DeleteStatusAsync(string statusId);

        Task<List<StatusListDto>> GetAllStatusesAsync();
    }
}
