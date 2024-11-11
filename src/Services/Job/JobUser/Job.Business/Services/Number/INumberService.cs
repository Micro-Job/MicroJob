using Job.Business.Dtos.NumberDtos;

namespace Job.Business.Services.Number
{
    public interface INumberService
    {
        Task<ICollection<Core.Entities.Number>> CreateBulkNumberAsync(ICollection<NumberCreateDto> numberCreateDtos);
        Task CreateNumberAsync(NumberCreateDto numberCreateDto);
        Task UpdateNumberAsync(ICollection<NumberUpdateDto> dtos);
    }
}