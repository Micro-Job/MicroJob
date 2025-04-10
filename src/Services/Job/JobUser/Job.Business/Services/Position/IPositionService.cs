using Job.Business.Dtos.Position;
using Job.Business.Dtos.PositionDtos;

namespace Job.Business.Services.Position;

public interface IPositionService
{
    Task<Guid> GetOrCreatePositionAsync(string? positionName, Guid? selectedPositionId, Guid? parentPositionId);
    Task<List<PositionDto>> GetAllPositionsAsync();

    Task<List<PositionListDto>> GetMainPositionsAsync();
    Task<List<PositionListDto>> GetSubPositionsAsync(string parentId);
}
