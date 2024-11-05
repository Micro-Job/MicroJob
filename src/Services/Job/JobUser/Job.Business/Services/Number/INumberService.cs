using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.NumberDtos;

namespace Job.Business.Services.Number
{
    public interface INumberService
    {
        Task CreateNumberAsync(NumberCreateDto numberCreateDto);
        Task UpdateNumberAsync(string id,NumberUpdateDto numberUpdateDto);
    }
}