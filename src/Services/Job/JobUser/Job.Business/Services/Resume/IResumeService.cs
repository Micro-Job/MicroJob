﻿using Job.Business.Dtos.ResumeDtos;

namespace Job.Business.Services.Resume
{
    public interface IResumeService
    {
        Task CreateResumeAsync(ResumeCreateDto resumeCreateDto,
        ResumeCreateListsDto resumeCreateListsDto);
        Task UpdateResumeAsync(ResumeUpdateDto resumeUpdateDto);
        //Task<IEnumerable<ResumeListDto>> GetAllResumeAsync();
        Task<ResumeDetailItemDto> GetByIdResumeAsync();
    }
}