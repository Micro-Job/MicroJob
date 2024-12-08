using Job.Business.Dtos.NumberDtos;

namespace Job.Business.Services.Number
{
    public interface INumberService
    {
        Task<List<Core.Entities.Number>> CreateBulkNumberAsync(ICollection<NumberCreateDto> numberCreateDtos,Guid resumeId);
        Task CreateNumberAsync(NumberCreateDto numberCreateDto);
        Task UpdateNumberAsync(ICollection<NumberUpdateDto> dtos);
    }
}