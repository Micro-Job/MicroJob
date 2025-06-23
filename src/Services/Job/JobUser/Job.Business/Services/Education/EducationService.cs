using Job.Business.Dtos.EducationDtos;
using Job.DAL.Contexts;
using SharedLibrary.Exceptions;

namespace Job.Business.Services.Education;

public class EducationService(JobDbContext context) : IEducationService
{
    public async Task<ICollection<Core.Entities.Education>> CreateBulkEducationAsync(ICollection<EducationCreateDto> dtos, Guid resumeId)
    {
        var educationsToAdd = dtos.Select(dto => MapEducationDtoToEntityForCreate(dto, resumeId)).ToList();

        await context.Educations.AddRangeAsync(educationsToAdd);
        await context.SaveChangesAsync();
        return educationsToAdd;
    }

    public async Task<ICollection<Core.Entities.Education>> UpdateBulkEducationAsync(ICollection<EducationUpdateDto> updateEducationDtos, ICollection<Core.Entities.Education> existEducations, Guid resumeId)
    {
        List<Core.Entities.Education>? newEducations = null;  // Yeni əlavə olunan təhsilləri saxlayır
        var allEducations = new List<Core.Entities.Education>();  // Geri qaytarılacaq bütün təhsilləri saxlayır

        var dtoIds = updateEducationDtos
           .Where(dto => !string.IsNullOrEmpty(dto.Id)) 
           .Select(dto => Guid.Parse(dto.Id!))
           .ToHashSet();

        foreach (var updatedEducation in updateEducationDtos)
        {
            if (updatedEducation.Id is not null) // Id null deyilse, bu təhsil artıq mövcuddur və yenilənməlidir
            {
                var existingEducation = existEducations.FirstOrDefault(e => e.Id == Guid.Parse(updatedEducation.Id));

                if (existingEducation is not null)
                    MapEducationDtoToEntityForUpdate(existingEducation, updatedEducation);
                else
                    throw new NotFoundException();

                allEducations.Add(existingEducation);
            }
            else // Id null-dursa, bu yeni təhsildir və əlavə olunmalıdır
            {
                newEducations ??= [];
                var newEducation = MapEducationDtoToEntityForCreate(new EducationCreateDto
                {
                    InstitutionName = updatedEducation.InstitutionName,
                    Profession = updatedEducation.Profession,
                    StartDate = updatedEducation.StartDate,
                    EndDate = updatedEducation.IsCurrentEducation ? null : updatedEducation.EndDate,
                    IsCurrentEducation = updatedEducation.IsCurrentEducation,
                    ProfessionDegree = updatedEducation.ProfessionDegree
                }, resumeId);

                newEducations.Add(newEducation);
            }
        }

        // Request-də göndərilən təhsil ID-ləri ilə mövcud olan təhsil ID-lərini müqayisə edib silinməli olanları tapırıq
        var educationsToRemove = existEducations.Where(e => !dtoIds.Contains(e.Id)).ToList();

        if (educationsToRemove.Count != 0)
            context.Educations.RemoveRange(educationsToRemove);

        if (newEducations?.Count > 0)
        {
            await context.Educations.AddRangeAsync(newEducations);
            allEducations.AddRange(newEducations);
        }

        return allEducations;
    }


    private static Core.Entities.Education MapEducationDtoToEntityForCreate(EducationCreateDto dto, Guid resumeId)
    {
        return new Core.Entities.Education
        {
            ResumeId = resumeId,
            InstitutionName = dto.InstitutionName,
            Profession = dto.Profession,
            StartDate = dto.StartDate,
            EndDate = dto.IsCurrentEducation ? null : dto.EndDate,
            IsCurrentEducation = dto.IsCurrentEducation,
            ProfessionDegree = dto.ProfessionDegree,
        };
    }

    private static void MapEducationDtoToEntityForUpdate(Core.Entities.Education education, EducationUpdateDto dto)
    {
        education.InstitutionName = dto.InstitutionName;
        education.Profession = dto.Profession;
        education.StartDate = dto.StartDate;
        education.EndDate = dto.IsCurrentEducation ? null : dto.EndDate;
        education.IsCurrentEducation = dto.IsCurrentEducation;
        education.ProfessionDegree = dto.ProfessionDegree;
    }
}
