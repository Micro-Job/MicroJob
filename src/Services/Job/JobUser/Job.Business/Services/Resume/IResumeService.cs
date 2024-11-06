using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Job.Business.Dtos.ResumeDtos;

namespace Job.Business.Services.Resume
{
    public interface IResumeService
    {
        Task CreateResumeAsync(ResumeCreateDto resumeCreateDto);
        Task UpdateResumeAsync(ResumeUpdateDto resumeUpdateDto);
        Task<IEnumerable<ResumeListDto>> GetAllResumeAsync();
        Task<ResumeDetailItemDto> GetByIdResumeAsync(string id);
    }
}

