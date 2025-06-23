using Job.Business.Dtos.Position;
using Job.Business.Dtos.PositionDtos;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;

namespace Job.Business.Services.Position;

public class PositionService(JobDbContext _context) : IPositionService
{
    /// <summary>
    /// Sadəcə IsActive-i true olan positionları qaytarır.
    /// </summary>
    public async Task<List<PositionDto>> GetAllPositionsAsync()
    {
        var positions = await _context.Positions
            .Where(p => p.ParentPositionId == null && p.IsActive)
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name,
                IsActive = p.IsActive,
                ParentPositionId = p.ParentPositionId,
                SubPositions = p.SubPositions != null ? p.SubPositions.Select(sp => new PositionDto
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    IsActive = sp.IsActive,
                    ParentPositionId = sp.ParentPositionId
                }).ToList() : new List<PositionDto>()
            })
            .AsNoTracking()
            .ToListAsync();

        return positions;
    }

    public async Task<List<PositionListDto>> GetMainPositionsAsync()
    {
        var positions = await _context.Positions.Where(x => x.IsActive && x.ParentPositionId == null)
        .Select(x => new PositionListDto
        {
            Id = x.Id,
            Name = x.Name
        })
        .AsNoTracking()
        .ToListAsync();

        return positions;
    }

    public async Task<List<PositionListDto>> GetSubPositionsAsync(string parentId)
    {
        var positions = await _context.Positions.Where(x => x.IsActive && x.ParentPositionId == Guid.Parse(parentId))
        .Select(x => new PositionListDto
        {
            Id = x.Id,
            Name = x.Name
        })
        .AsNoTracking()
        .ToListAsync();

        return positions;
    }

    /// <summary>
    /// Position varsa onun id-sini qaytarır, yoxdursa yaradır və yenisinin id-sini geriyə qaytarır.
    /// </summary>
    public async Task<Guid> GetOrCreatePositionAsync(string? positionName, Guid? selectedPositionId, Guid? parentPositionId)
    {
        if (selectedPositionId != null)
        {
            var isPositionExist = await _context.Positions.AnyAsync(p => p.Id == selectedPositionId);

            if (isPositionExist)
                return selectedPositionId.Value;
            else
                throw new NotFoundException();
        }

        else
        {
            if (!string.IsNullOrEmpty(positionName))
            {
                positionName = positionName.Trim();

                var positionId = await _context.Positions.Where(p => p.Name == positionName).Select(x => x.Id).FirstOrDefaultAsync();

                if (positionId != Guid.Empty) return positionId;

                var newPosition = new Core.Entities.Position
                {
                    Name = positionName,
                    IsActive = false,
                    ParentPositionId = parentPositionId
                };

                await _context.Positions.AddAsync(newPosition);
                await _context.SaveChangesAsync();
                return newPosition.Id;
            }
        }
        return Guid.Empty;
    }
}