using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Services.Experience;

public class ExperienceService(JobDbContext context) : IExperienceService
{
    public async Task<ICollection<Core.Entities.Experience>> CreateBulkExperienceAsync(
        ICollection<ExperienceCreateDto> dtos,
        Guid resumeId
    )
    {
        var experiencesToAdd = dtos.Select(dto => MapExperienceDtoToEntityForCreate(dto, resumeId))
            .ToList();

        await context.Experiences.AddRangeAsync(experiencesToAdd);
        await context.SaveChangesAsync();

        return experiencesToAdd;
    }

    public async Task CreateExperienceAsync(
        ExperienceCreateDto dto,
        Guid resumeId,
        bool saveChanges = true
    )
    {
        var experience = MapExperienceDtoToEntityForCreate(dto, resumeId);

        await context.Experiences.AddAsync(experience);

        if (saveChanges)
            await context.SaveChangesAsync();
    }

    public async Task<ICollection<Core.Entities.Experience>> UpdateBulkExperienceAsync(
        ICollection<ExperienceUpdateDto> dtos,
        Guid resumeId
    )
    {
        var experiencesToUpdate = new List<Core.Entities.Experience>();

        foreach (var dto in dtos)
        {
            var experience = await UpdateExperienceAsync(dto, resumeId, saveChanges: false);

            experiencesToUpdate.Add(experience);
        }

        await context.SaveChangesAsync();

        return experiencesToUpdate;
    }

    public async Task<Core.Entities.Experience> UpdateExperienceAsync(
        ExperienceUpdateDto dto,
        Guid resumeId,
        bool saveChanges = true
    )
    {
        var experience =
            await context.Experiences.FirstOrDefaultAsync(e =>
                e.ResumeId == resumeId && e.OrganizationName == dto.OrganizationName
            ) ?? throw new NotFoundException<Core.Entities.Experience>();

        MapExperienceDtoToEntityForUpdate(experience, dto);

        if (saveChanges)
            await context.SaveChangesAsync();

        return experience;
    }

    private static Core.Entities.Experience MapExperienceDtoToEntityForCreate(
        ExperienceCreateDto dto,
        Guid resumeId
    )
    {
        return new Core.Entities.Experience
        {
            ResumeId = resumeId,
            OrganizationName = dto.OrganizationName,
            PositionName = dto.PositionName,
            PositionDescription = dto.PositionDescription,
            StartDate = dto.StartDate,
            EndDate = dto.IsCurrentOrganization ? null : dto.EndDate,
            IsCurrentOrganization = dto.IsCurrentOrganization,
        };
    }

    private static void MapExperienceDtoToEntityForUpdate(
        Core.Entities.Experience experience,
        ExperienceUpdateDto dto
    )
    {
        experience.OrganizationName = dto.OrganizationName;
        experience.PositionName = dto.PositionName;
        experience.PositionDescription = dto.PositionDescription;
        experience.StartDate = dto.StartDate;
        experience.EndDate = dto.IsCurrentOrganization ? null : dto.EndDate;
        experience.IsCurrentOrganization = dto.IsCurrentOrganization;
    }
}
