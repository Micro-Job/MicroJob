using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.EducationDtos;

namespace Job.Business.Services.Education
{
    public interface IEducationService
    {
        Task CreateEducationAsync(EducationCreateDto dto);
        Task UpdateEducationAsync(string id, EducationUpdateDto dto);
    }
}