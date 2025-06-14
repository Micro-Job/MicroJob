using Job.Business.Dtos.NumberDtos;

namespace Job.Business.Services.Number;

public interface INumberService
{
    List<Core.Entities.Number> CreateBulkNumber(ICollection<NumberCreateDto> numbersDto, Guid resumeId, string? mainNumber = null);
    Task<List<Core.Entities.Number>> UpdateBulkNumberAsync(ICollection<NumberUpdateDto> numberUpdateDtos, ICollection<Core.Entities.Number> existingNumbers, Guid resumeId, string? mainNumber = null);
}