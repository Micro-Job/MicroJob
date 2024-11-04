using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.PersonDtos;

namespace Job.Business.Services.Person
{
    public interface IPersonService
    {
        Task CreateAsync(PersonCreateDto dto);
        Task UpdateAsync(PersonUpdateDto dto);
        Task<IEnumerable<PersonListDto>> GetAllAsync();
        Task<PersonDetailItemDto> GetByIdAsync(Guid id);
    }
}