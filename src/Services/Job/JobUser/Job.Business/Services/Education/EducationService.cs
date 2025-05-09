﻿using Job.Business.Dtos.EducationDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Helpers;

namespace Job.Business.Services.Education
{
    public class EducationService(JobDbContext context) : IEducationService
    {
        public async Task<ICollection<Core.Entities.Education>> CreateBulkEducationAsync(ICollection<EducationCreateDto> dtos,Guid resumeId)
        {
            var educationsToAdd = dtos.Select(dto =>
                    MapEducationDtoToEntityForCreate(dto, resumeId)
                )
                .ToList();

            await context.Educations.AddRangeAsync(educationsToAdd);

            return educationsToAdd;
        }

        public async Task CreateEducationAsync(EducationCreateDto dto, Guid resumeId)
        {
            var education = MapEducationDtoToEntityForCreate(dto, resumeId);

            await context.Educations.AddAsync(education);
        }

        //TODO : Burada kod optimizasiyasına ehtiyyac var 
        public async Task<ICollection<Core.Entities.Education>> UpdateBulkEducationAsync(ICollection<EducationUpdateDto> dtos, Guid resumeId)
        {
            var institutionNames = dtos.Select(d => d.InstitutionName).ToList();

            var educations = await context.Educations
                .Where(e => e.ResumeId == resumeId && institutionNames.Contains(e.InstitutionName))
                .ToListAsync();

            if (!educations.Any())
                throw new NotFoundException<Core.Entities.Education>(MessageHelper.GetMessage("NOT_FOUND"));

            educations = educations
                .Select(education =>
                {
                    var dto = dtos.FirstOrDefault(d => d.InstitutionName == education.InstitutionName);
                    if (dto != null)
                    {
                        MapEducationDtoToEntityForUpdate(education, dto);
                    }
                    return education;
                })
                .ToList();

            await context.SaveChangesAsync();

            return educations;
        }


        public async Task UpdateEducationAsync(EducationUpdateDto dto)
        {
            var education =
                await context.Educations.FindAsync(dto.Id)
                ?? throw new NotFoundException<Core.Entities.Education>(MessageHelper.GetMessage("NOT_FOUND"));

            MapEducationDtoToEntityForUpdate(education, dto);
        }

        private Core.Entities.Education MapEducationDtoToEntityForCreate(EducationCreateDto dto,Guid resumeId)
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

        private void MapEducationDtoToEntityForUpdate(Core.Entities.Education education,EducationUpdateDto dto)
        {
            education.InstitutionName = dto.InstitutionName;
            education.Profession = dto.Profession;
            education.StartDate = dto.StartDate;
            education.EndDate = dto.IsCurrentEducation ? null : dto.EndDate;
            education.IsCurrentEducation = dto.IsCurrentEducation;
            education.ProfessionDegree = dto.ProfessionDegree;
        }
    }
}
