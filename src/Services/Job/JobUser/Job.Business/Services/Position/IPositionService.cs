using Job.Business.Dtos.PositionDtos;

namespace Job.Business.Services.Position;

public interface IPositionService
{
    Task<Guid> GetOrCreatePositionAsync(string? positionName, Guid? selectedPositionId, Guid? parentPositionId = null);
    Task<List<PositionDto>> GetAllPositionsAsync();
}
