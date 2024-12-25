using Job.Business.Dtos.NumberDtos;

namespace Job.Business.Services.Number
{
    public interface INumberService
    {
        Task<List<Core.Entities.Number>> CreateBulkNumberAsync(ICollection<NumberCreateDto> numberCreateDtos,Guid resumeId);
        Task<List<Core.Entities.Number>> UpdateBulkNumberAsync(ICollection<NumberUpdateDto> numberUpdateDtos,Guid resumeId);
        Task<Core.Entities.Number> CreateNumberAsync(NumberCreateDto numberCreateDto, Guid resumeId);
        Task UpdateNumberAsync(NumberUpdateDto numberUpdateDto);
    }
}