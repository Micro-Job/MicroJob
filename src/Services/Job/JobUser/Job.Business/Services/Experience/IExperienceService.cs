using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.ExperienceDtos;

namespace Job.Business.Services.Experience
{
    public interface IExperienceService
    {
        Task CreateExperienceAsync(ExperienceCreateDto dto);
        Task UpdateExperienceAsync(string id,ExperienceUpdateDto dto);
    }
}