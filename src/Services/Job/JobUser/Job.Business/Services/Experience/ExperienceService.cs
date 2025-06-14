using Job.Business.Dtos.ExperienceDtos;
using Job.DAL.Contexts;

namespace Job.Business.Services.Experience;

public class ExperienceService(JobDbContext context) : IExperienceService
{
    public async Task<ICollection<Core.Entities.Experience>> CreateBulkExperienceAsync(ICollection<ExperienceCreateDto> dtos, Guid resumeId)
    {
        var experiencesToAdd = dtos.Select(dto => MapExperienceDtoToEntityForCreate(dto, resumeId)).ToList();

        await context.Experiences.AddRangeAsync(experiencesToAdd);

        return experiencesToAdd;
    }

    public async Task<ICollection<Core.Entities.Experience>> UpdateBulkExperienceAsync(ICollection<ExperienceUpdateDto> updateDtos, ICollection<Core.Entities.Experience> existingExperiences, Guid resumeId)
    {
        var resultList = new List<Core.Entities.Experience>();
        List<ExperienceCreateDto>? createDtos = null;

        var dtoIds = updateDtos
            .Where(x => !string.IsNullOrWhiteSpace(x.Id))
            .Select(x => Guid.Parse(x.Id!))
            .ToHashSet();

        foreach (var dto in updateDtos)
        {
            if (!string.IsNullOrWhiteSpace(dto.Id))
            {
                var existing = existingExperiences.FirstOrDefault(e => e.Id == Guid.Parse(dto.Id));
                if (existing != null)
                {
                    MapExperienceDtoToEntityForUpdate(existing, dto);
                    resultList.Add(existing);
                }
            }
            else
            {
                var createDto = new ExperienceCreateDto
                {
                    OrganizationName = dto.OrganizationName,
                    PositionName = dto.PositionName,
                    PositionDescription = dto.PositionDescription,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsCurrentOrganization = dto.IsCurrentOrganization
                };
                createDtos ??= [];
                createDtos.Add(createDto);
            }
        }

        var experiencesToRemove = existingExperiences.Where(e => !dtoIds.Contains(e.Id)).ToList();

        if (experiencesToRemove.Count != 0)
            context.Experiences.RemoveRange(experiencesToRemove);

        if (createDtos?.Count > 0)
        {
            var newExperiences = await CreateBulkExperienceAsync(createDtos, resumeId);
            resultList.AddRange(newExperiences);
        }

        return resultList;
    }


    private static Core.Entities.Experience MapExperienceDtoToEntityForCreate(ExperienceCreateDto dto, Guid resumeId)
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

    private static void MapExperienceDtoToEntityForUpdate(Core.Entities.Experience experience, ExperienceUpdateDto dto)
    {
        experience.OrganizationName = dto.OrganizationName;
        experience.PositionName = dto.PositionName;
        experience.PositionDescription = dto.PositionDescription;
        experience.StartDate = dto.StartDate;
        experience.EndDate = dto.IsCurrentOrganization ? null : dto.EndDate;
        experience.IsCurrentOrganization = dto.IsCurrentOrganization;
    }
}
